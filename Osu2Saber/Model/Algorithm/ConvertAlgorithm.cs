using Osu2Saber.Model.Json;
using osuBMParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Osu2Saber.Model.Algorithm
{
    public class ConvertAlgorithm
    {
        public static bool IgnoreHitSlider = false;
        public static bool NoDirectionAndPlacement = false;
        public static bool GenerateLight = true;
        public static bool AllBottom = false;
        public static bool AllUpDown = false;
        public static bool GenerateAudio = false;
        public static bool GenerateAllStrobe = true;
        public static bool RandomizeColor = false;
        public static bool OnlyMakeTimingNote = false;
        public static bool CreateDouble = true;
        public static bool GenerateGallops = false;
        public static bool AllTopUp = false;

        protected const float OsuScreenXMax = 512, OsuScreenYMax = 384;

        public static double EnoughIntervalBetweenNotes = 0.250f;
        public static string PatternToUse = "All";

        protected Beatmap org;
        protected SaberBeatmap dst;

        protected List<Event> events = new List<Event>();
        protected List<Obstacle> obstacles = new List<Obstacle>();
        protected List<Note> notes = new List<Note>();
        protected int bpm;
        protected int offset;
        protected bool isMania = false;
        protected Event ev;
        protected int forceSpread = 0;
        protected int first = 0;
        protected List<Note> patternLoop;

        public List<Event> Events => events;
        public List<Obstacle> Obstacles => obstacles;
        public List<Note> Notes => notes;

        public List<int> UpCut = new List<int>() { 0, 4, 5 };
        public List<int> DownCut = new List<int>() { 1, 6, 7 };

        public ConvertAlgorithm(Beatmap osu, SaberBeatmap bs)
        {
            org = osu;
            dst = bs;
            bpm = dst._beatsPerMinute;
            offset = osu.TimingPoints[0].Offset;
            isMania = org.Mode == 3;
        }

        // You may override this method for better map generation
        public void Convert()
        {
            MakeHitObjects();
            if (!OnlyMakeTimingNote)
            {
                RemoveExcessNotes();
                MapReader();
                LogicChecker();
            }
            if(AllTopUp)
            {
                AllUpTop();
            }
            if (AllBottom)
            {
                BottomDisplacement();
            }
            if (AllUpDown)
            {
                UpDown();
            }
        }

        #region Process for placing notes
        void MakeHitObjects()
        {
            foreach (var obj in org.HitObjects)
            {
                if (obj.Time < EnoughIntervalBetweenNotes) continue;
                if(obj is HoldNote && !IgnoreHitSlider)
                {
                    var temp = (HoldNote)obj;

                    AddNote(temp.Time, temp.Position.x, temp.Position.y);
                }
                else if (obj is HitSlider && !IgnoreHitSlider)
                {
                    var temp = (HitSlider)obj;

                    var beatDuration = GetBeatDurationIn(temp.Time);
                    var sliderMultiplier = org.SliderMultiplier;
                    var durationMs = (int)Math.Round(temp.PixelLength / (100.0 * sliderMultiplier) * beatDuration);
                    var lastPos = temp.HitSliderSegments[temp.HitSliderSegments.Count - 1].position;

                    for (var i = 0; i < temp.Repeat; i++)
                    {
                        if (i == 0)
                        {
                            // add beginning and final point
                            AddNote(temp.Time, temp.Position.x, obj.Position.y);
                            AddNote(temp.Time + durationMs, lastPos.x, lastPos.y);
                        }
                        else if (i % 2 == 0)
                        {
                            AddNote(temp.Time + durationMs * (i + 1), temp.Position.x, obj.Position.y);
                        }
                        else
                        {
                            AddNote(temp.Time + durationMs * (i + 1), lastPos.x, lastPos.y);
                        }
                    }
                }
                else if (obj is HitSpinner temp)
                {
                    AddNote(temp.Time, temp.Position.x, temp.Position.y);

                    // this is just a nuisance at many times
                    //AddNote(temp.EndTime, temp.Position.x, temp.Position.y);
                }
                else  // is a HitCircle
                {
                    AddNote(obj.Time, obj.Position.x, obj.Position.y);
                }

            }
        }

        void AddNote(int timeMs, float posx, float posy)
        {
            var (line, layer) = DeterminePosition(posx, posy);
            var note = new Note(ConvertTime(timeMs), line, layer, 0, CutDirection.Any);
            notes.Add(note);
        }

        (int line, int layer) DeterminePosition(double x, double y)
        {
            if (x < 0) x = 0;
            else if (x > OsuScreenXMax) x = OsuScreenXMax;

            if (y < 0) y = 0;
            else if (y > OsuScreenYMax) y = OsuScreenYMax;

            // just map notes position to BS screen
            var line = (int)Math.Floor(x / (OsuScreenXMax + 1) * (double)Line.MaxNum);
            var layer = (int)Math.Floor(y / (OsuScreenYMax + 1) * (double)Layer.MaxNum);

            if (NoDirectionAndPlacement)
            {
                if (!isMania) return (0, 0);
                layer = DetermineLayerMania(line);
                return (line, layer);
            }

            if (isMania)
            {
                layer = DetermineLayerMania(line);
            }

            layer = SlideLayer(line, layer, y);

            if(first == 0)
            {
                layer = 0;
                first = 1;
            }

            return (line, layer);
        }

        int beforeLayerLeft = 0, beforeLayerRight = 0;
        int DetermineLayerMania(int line)
        {
            var diff = RandNumber(-1, 2);
            if (diff == 2)
            {
                diff = RandNumber(-1, 2);
            }
            if (DetermineColor(line) == NoteType.Red)
            {
                beforeLayerLeft += diff;
                beforeLayerLeft = beforeLayerLeft >= (int)Layer.MaxNum ? (int)Layer.Top : beforeLayerLeft;
                beforeLayerLeft = beforeLayerLeft < (int)0 ? (int)Layer.Bottom : beforeLayerLeft;
                return beforeLayerLeft;
            }
            else
            {
                beforeLayerRight += diff;
                beforeLayerRight = beforeLayerRight >= (int)Layer.MaxNum ? (int)Layer.Top : beforeLayerRight;
                beforeLayerRight = beforeLayerRight < (int)0 ? (int)Layer.Bottom : beforeLayerRight;
                return beforeLayerRight;
            }
        }

        int SlideLayer(int line, int layer, double y)
        {
            // don't want notes come to right front of our eyes so often
            if (layer != (int)Layer.Middle) return layer;
            if (line == (int)Line.Left || line == (int)Line.Right) return layer;

            // The larger this value is, the less likely notes appear in center middle.
            var fineSection = 8;
            var layerIdx = (int)Math.Floor(y / (OsuScreenYMax + 1) * fineSection);
            layerIdx = (int)Math.Floor(layerIdx / 1.5);
            if (layerIdx == fineSection / 2) return layer;
            if (layerIdx < fineSection / 2) return (int)Layer.Bottom;
            return (int)Layer.Top;
        }

        NoteType DetermineColor(int line)
        {
            if (line < 2)
            {
                return NoteType.Red;
            }
            else
            {
                return NoteType.Blue;
            }
        }

        public static int RandNumber(int Low, int High)
        {
            Random rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));

            int rnd = rndNum.Next(Low, High);

            return rnd;
        }

        void RemoveExcessNotes()
        {
            notes = notes.OrderBy(note => note._time).ToList();

            notes = RemoveExcessNotes(notes);
            
            if (CreateDouble && !GenerateGallops) // Stream + Double
            {
                for (int i = notes.Count() - 1; i > 1; i--)
                {
                    if (notes[i]._time - notes[i - 1]._time >= -0.01 && notes[i]._time - notes[i - 1]._time <= 0.01 && notes[i - 1]._time - notes[i - 2]._time <= EnoughIntervalBetweenNotes)
                    {
                        notes.Remove(notes[i]);
                    }
                }
                for (int i = notes.Count() - 2; i > 0; i--)
                {
                    if (notes[i]._time - notes[i - 1]._time >= -0.01 && notes[i]._time - notes[i - 1]._time <= 0.01 && notes[i + 1]._time - notes[i]._time <= EnoughIntervalBetweenNotes)
                    {
                        notes.Remove(notes[i]);
                    }
                }
            }
        }

        List<Note> RemoveExcessNotes(List<Note> notes)
        {
            double lastTime = -100;
            var keepNotes = new List<Note>();
            var dbl = false;

            foreach (var note in notes)
            {
                double timeGap = note._time - lastTime;
                if(timeGap <= 0.01 && timeGap >= -0.01 && CreateDouble && dbl == false)
                {
                    dbl = true;
                    keepNotes.Add(note);
                }
                else if (timeGap <= 0.01 && timeGap >= -0.01 && CreateDouble && dbl == true)
                {
                    continue;
                }
                else if (timeGap >= EnoughIntervalBetweenNotes)
                {
                    keepNotes.Add(note);
                    lastTime = note._time;
                    dbl = false;
                }
            }
            return keepNotes;
        }

        double GetBeatDurationIn(int timeMs)
        {
            var baseMsPerBeat = org.TimingPoints[0].MsPerBeat;
            var fac = 1.0;
            foreach (var point in org.TimingPoints)
            {
                if (point.MsPerBeat > 0)
                {
                    baseMsPerBeat = point.MsPerBeat;
                    fac = 1;
                }
                else
                    fac = -point.MsPerBeat / 100.0;

                // TimingPoints is supposed to be sorted by default
                if (point.Offset >= timeMs)
                {
                    break;
                }
            }
            return baseMsPerBeat * fac;
        }

        protected double ConvertTime(int timeMs)
        {
            var unit = 60.0 / bpm / 8.0;
            var sectionIdx = (int)Math.Round(((timeMs) / 1000.0 / unit));
            if(isMania)
            {
                return Math.Round(sectionIdx / 8.0, 3, MidpointRounding.AwayFromZero) + 0.125;
            }
            else
            {
                return Math.Round(sectionIdx / 8.0, 3, MidpointRounding.AwayFromZero);
            }
        }

        protected int ConvertBeat(double timeBeat)
        {
            return (int)Math.Round(timeBeat / bpm * 60 * 1000);
        }

        #endregion

        #region Process for logic

        void LogicChecker()
        {
            notes = notes.OrderBy(note => note._time).ToList();

            //Fix fused hitbox issue
            for (int i = 0; i < notes.Count() - 1; i++)
            {
                Note n = notes[i];

                if (notes[i + 1]._time - n._time <= 0.02 && notes[i + 1]._time - n._time >= -0.02 && (n._lineLayer == notes[i + 1]._lineLayer || n._lineLayer == notes[i + 1]._lineLayer - 1 || n._lineLayer == notes[i + 1]._lineLayer + 1) && (n._lineIndex == notes[i + 1]._lineIndex || n._lineIndex == notes[i + 1]._lineIndex + 1 || n._lineIndex == notes[i + 1]._lineIndex - 1))
                {
                    if (n._type == 0 && n._lineIndex <= 1)
                    {
                        notes[i + 1]._lineIndex = n._lineIndex + 2;
                    }
                    else if (n._type == 1 && n._lineIndex >= 2)
                    {
                        notes[i + 1]._lineIndex = n._lineIndex - 2;
                    }
                    else if (n._type == 1 && notes[i + 1]._lineIndex <= 1)
                    {
                        n._lineIndex = notes[i + 1]._lineIndex + 2;
                    }
                    else if (n._type == 0 && notes[i + 1]._lineIndex >= 2)
                    {
                        n._lineIndex = notes[i + 1]._lineIndex - 2;
                    }
                }
            }

            // Make the map flow better
            for (int i = 0; i < notes.Count() - 1; i++)
            {
                Note n = notes[i];

                if (notes[i + 1]._time - n._time <= 0.02 && notes[i + 1]._time - n._time >= -0.02)
                {
                    switch (n._cutDirection)
                    {
                        case 0: 
                            if(notes[i + 1]._lineIndex == n._lineIndex)
                            {
                                if (n._type == 0 && n._lineIndex != 0 && n._lineLayer != 1)
                                {
                                    n._lineIndex--;
                                }
                                else if (n._type == 1 && n._lineIndex != 3 && n._lineLayer != 1)
                                {
                                    n._lineIndex++;
                                }
                                else if(n._type == 0 && n._lineIndex == 0 && notes[i + 1]._lineLayer != 1)
                                {
                                    notes[i + 1]._lineIndex++;
                                }
                                else if(n._type == 1 && n._lineIndex == 3 && notes[i + 1]._lineLayer != 1)
                                {
                                    notes[i + 1]._lineIndex--;
                                }
                            }
                            break;
                        case 1:
                            if (notes[i + 1]._lineIndex == n._lineIndex)
                            {
                                if (n._type == 0 && n._lineIndex != 0 && n._lineLayer != 1)
                                {
                                    n._lineIndex--;
                                }
                                else if (n._type == 1 && n._lineIndex != 3 && n._lineLayer != 1)
                                {
                                    n._lineIndex++;
                                }
                                else if (n._type == 0 && n._lineIndex == 0 && notes[i + 1]._lineLayer != 1)
                                {
                                    notes[i + 1]._lineIndex++;
                                }
                                else if (n._type == 1 && n._lineIndex == 3 && notes[i + 1]._lineLayer != 1)
                                {
                                    notes[i + 1]._lineIndex--;
                                }
                            }
                            break;
                        case 2:
                            if(notes[i + 1]._lineLayer == n._lineLayer)
                            {
                                if(n._lineLayer == 2 || n._lineLayer == 1)
                                {
                                    n._lineLayer--;
                                }
                                else
                                {
                                    n._lineLayer++;
                                }
                            }
                            break;
                        case 3:
                            if (notes[i + 1]._lineLayer == n._lineLayer)
                            {
                                if (n._lineLayer == 2 || n._lineLayer == 1)
                                {
                                    n._lineLayer--;
                                }
                                else
                                {
                                    n._lineLayer++;
                                }
                            }
                            break;
                        case 4:
                            if ((notes[i + 1]._lineIndex == n._lineIndex - 1 && notes[i + 1]._lineLayer == n._lineLayer + 1) || (notes[i + 1]._lineIndex == n._lineIndex + 1 && notes[i + 1]._lineLayer == n._lineLayer - 1) || (notes[i + 1]._lineIndex == n._lineIndex - 1 && notes[i + 1]._lineLayer == n._lineLayer) || (notes[i + 1]._lineIndex == n._lineIndex && notes[i + 1]._lineLayer == n._lineLayer + 1) || (notes[i + 1]._lineIndex == n._lineIndex && notes[i + 1]._lineLayer == n._lineLayer - 1) || (notes[i + 1]._lineIndex == n._lineIndex + 1 && notes[i + 1]._lineLayer == n._lineLayer))
                            {
                                if((notes[i + 1]._cutDirection == 4 || notes[i + 1]._cutDirection == 6) && n._lineLayer != 0 && n._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex = n._lineIndex - 1;
                                    notes[i + 1]._lineLayer = n._lineLayer - 1;
                                    if(notes[i + 1]._lineLayer == 1)
                                    {
                                        notes[i + 1]._lineLayer--;
                                    }
                                }
                                else if ((notes[i + 1]._cutDirection == 4 || notes[i + 1]._cutDirection == 6) && notes[i + 1]._lineLayer != 2 && notes[i + 1]._lineIndex != 3)
                                {
                                    n._lineIndex = notes[i + 1]._lineIndex + 1;
                                    n._lineLayer = notes[i + 1]._lineLayer + 1;
                                    if (n._lineLayer == 1)
                                    {
                                        n._lineLayer++;
                                    }
                                }
                                else if(n._type == 1 && n._lineIndex != 3)
                                {
                                    n._lineIndex++;
                                }
                                else if (notes[i + 1]._type == 0 && notes[i + 1]._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex--;
                                }
                                else if(n._type == 0 && n._lineIndex != 0)
                                {
                                    n._lineIndex--;
                                }
                            }
                            break;
                        case 5:
                            if ((notes[i + 1]._lineIndex == n._lineIndex - 1 && notes[i + 1]._lineLayer == n._lineLayer - 1) || (notes[i + 1]._lineIndex == n._lineIndex + 1 && notes[i + 1]._lineLayer == n._lineLayer + 1) || (notes[i + 1]._lineIndex == n._lineIndex - 1 && notes[i + 1]._lineLayer == n._lineLayer) || (notes[i + 1]._lineIndex == n._lineIndex && notes[i + 1]._lineLayer == n._lineLayer + 1) || (notes[i + 1]._lineIndex == n._lineIndex && notes[i + 1]._lineLayer == n._lineLayer - 1) || (notes[i + 1]._lineIndex == n._lineIndex + 1 && notes[i + 1]._lineLayer == n._lineLayer))
                            {
                                if ((notes[i + 1]._cutDirection == 5 || notes[i + 1]._cutDirection == 7) && n._lineLayer != 0 && n._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex = n._lineIndex - 1;
                                    notes[i + 1]._lineLayer = n._lineLayer - 1;
                                    if (notes[i + 1]._lineLayer == 1)
                                    {
                                        notes[i + 1]._lineLayer--;
                                    }
                                }
                                else if ((notes[i + 1]._cutDirection == 5 || notes[i + 1]._cutDirection == 7) && notes[i + 1]._lineLayer != 2 && notes[i + 1]._lineIndex != 3)
                                {
                                    n._lineIndex = notes[i + 1]._lineIndex + 1;
                                    n._lineLayer = notes[i + 1]._lineLayer + 1;
                                    if (n._lineLayer == 1)
                                    {
                                        n._lineLayer++;
                                    }
                                }
                                else if (n._type == 1 && n._lineIndex != 3)
                                {
                                    n._lineIndex++;
                                }
                                else if (notes[i + 1]._type == 0 && notes[i + 1]._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex--;
                                }
                                else if (n._type == 0 && n._lineIndex != 0)
                                {
                                    n._lineIndex--;
                                }
                            }
                            break;
                        case 6:
                            if ((notes[i + 1]._lineIndex == n._lineIndex - 1 && notes[i + 1]._lineLayer == n._lineLayer + 1) || (notes[i + 1]._lineIndex == n._lineIndex + 1 && notes[i + 1]._lineLayer == n._lineLayer - 1) || (notes[i + 1]._lineIndex == n._lineIndex - 1 && notes[i + 1]._lineLayer == n._lineLayer) || (notes[i + 1]._lineIndex == n._lineIndex && notes[i + 1]._lineLayer == n._lineLayer + 1) || (notes[i + 1]._lineIndex == n._lineIndex && notes[i + 1]._lineLayer == n._lineLayer - 1) || (notes[i + 1]._lineIndex == n._lineIndex + 1 && notes[i + 1]._lineLayer == n._lineLayer))
                            {
                                if ((notes[i + 1]._cutDirection == 4 || notes[i + 1]._cutDirection == 6) && n._lineLayer != 0 && n._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex = n._lineIndex - 1;
                                    notes[i + 1]._lineLayer = n._lineLayer - 1;
                                    if (notes[i + 1]._lineLayer == 1)
                                    {
                                        notes[i + 1]._lineLayer--;
                                    }
                                }
                                else if ((notes[i + 1]._cutDirection == 4 || notes[i + 1]._cutDirection == 6) && notes[i + 1]._lineLayer != 2 && notes[i + 1]._lineIndex != 3)
                                {
                                    n._lineIndex = notes[i + 1]._lineIndex + 1;
                                    n._lineLayer = notes[i + 1]._lineLayer + 1;
                                    if (n._lineLayer == 1)
                                    {
                                        n._lineLayer++;
                                    }
                                }
                                else if (n._type == 1 && n._lineIndex != 3)
                                {
                                    n._lineIndex++;
                                }
                                else if (notes[i + 1]._type == 0 && notes[i + 1]._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex--;
                                }
                                else if (n._type == 0 && n._lineIndex != 0)
                                {
                                    n._lineIndex--;
                                }
                            }
                            break;
                        case 7:
                            if ((notes[i + 1]._lineIndex == n._lineIndex - 1 && notes[i + 1]._lineLayer == n._lineLayer - 1) || (notes[i + 1]._lineIndex == n._lineIndex + 1 && notes[i + 1]._lineLayer == n._lineLayer + 1) || (notes[i + 1]._lineIndex == n._lineIndex - 1 && notes[i + 1]._lineLayer == n._lineLayer) || (notes[i + 1]._lineIndex == n._lineIndex && notes[i + 1]._lineLayer == n._lineLayer + 1) || (notes[i + 1]._lineIndex == n._lineIndex && notes[i + 1]._lineLayer == n._lineLayer - 1) || (notes[i + 1]._lineIndex == n._lineIndex + 1 && notes[i + 1]._lineLayer == n._lineLayer))
                            {
                                if ((notes[i + 1]._cutDirection == 5 || notes[i + 1]._cutDirection == 7) && n._lineLayer != 0 && n._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex = n._lineIndex - 1;
                                    notes[i + 1]._lineLayer = n._lineLayer - 1;
                                    if (notes[i + 1]._lineLayer == 1)
                                    {
                                        notes[i + 1]._lineLayer--;
                                    }
                                }
                                else if ((notes[i + 1]._cutDirection == 5 || notes[i + 1]._cutDirection == 7) && notes[i + 1]._lineLayer != 2 && notes[i + 1]._lineIndex != 3)
                                {
                                    n._lineIndex = notes[i + 1]._lineIndex + 1;
                                    n._lineLayer = notes[i + 1]._lineLayer + 1;
                                    if (n._lineLayer == 1)
                                    {
                                        n._lineLayer++;
                                    }
                                }
                                else if (n._type == 1 && n._lineIndex != 3)
                                {
                                    n._lineIndex++;
                                }
                                else if (notes[i + 1]._type == 0 && notes[i + 1]._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex--;
                                }
                                else if (n._type == 0 && n._lineIndex != 0)
                                {
                                    n._lineIndex--;
                                }
                            }
                            break;
                        case 8:
                            break;
                    }
                }
            }

            //Fix vision
            for (int i = 0; i < notes.Count() - 1; i++)
            {
                Note n = notes[i];

                if ((n._lineIndex == 1 || n._lineIndex == 2) && n._lineLayer == 1)
                {
                    if (n._cutDirection == 0 || n._cutDirection == 4 || n._cutDirection == 5)
                    {
                        n._lineLayer = 2;
                    }
                    else if (n._cutDirection == 1 || n._cutDirection == 6 || n._cutDirection == 7)
                    {
                        n._lineLayer = 0;
                    }
                    else
                    {
                        n._lineLayer = 0;
                    }
                }
            }

            //Swap pickles
            for (int i = 0; i < notes.Count() - 1; i++)
            {
                if (notes[i + 1]._time - notes[i]._time <= 0.01 && notes[i + 1]._time - notes[i]._time >= -0.01)
                {
                    if ((notes[i]._lineIndex > notes[i + 1]._lineIndex + 1 && notes[i]._type == 0) || (notes[i]._lineIndex < notes[i + 1]._lineIndex - 1 && notes[i]._type == 1))
                    {
                        int tempo = notes[i]._lineIndex;
                        notes[i]._lineIndex = notes[i + 1]._lineIndex;
                        notes[i + 1]._lineIndex = tempo;

                        tempo = notes[i]._lineLayer;
                        notes[i]._lineLayer = notes[i + 1]._lineLayer;
                        notes[i + 1]._lineLayer = tempo;
                    }
                }
            }

            Note lastBlue = new Note(0, 0, 0, 0, 0);
            Note lastRed = new Note(0, 0, 0, 0, 0);

            //Fix fused hitbox issue again
            for (int i = 0; i < notes.Count() - 1; i++)
            {
                Note n = notes[i];

                if (notes[i + 1]._time - n._time <= 0.02 && notes[i + 1]._time - n._time >= -0.02 && n._lineLayer == notes[i + 1]._lineLayer && n._lineIndex == notes[i + 1]._lineIndex)
                {
                    if(n._type == 0 && n._lineIndex >= 2)
                    {
                        n._lineIndex -= 2;
                    }
                    else if(n._type == 1 && n._lineIndex <= 1)
                    {
                        n._lineIndex += 2;
                    }
                    else if (notes[i + 1]._type == 0 && notes[i + 1]._lineIndex >= 2)
                    {
                        notes[i + 1]._lineIndex -= 2;
                    }
                    else if (notes[i + 1]._type == 1 && notes[i + 1]._lineIndex <= 1)
                    {
                        notes[i + 1]._lineIndex += 2;
                    }
                }
            }

            //Fix new pickles hitbox angle issue
            for (int i = 0; i < notes.Count() - 1; i++)
            {
                Note n = notes[i];

                if (notes[i + 1]._time - n._time <= 0.02 && notes[i + 1]._time - n._time >= -0.02 && n._lineLayer == notes[i + 1]._lineLayer && (n._lineIndex == notes[i + 1]._lineIndex - 1 || n._lineIndex == notes[i + 1]._lineIndex + 1))
                {
                    if(n._cutDirection == 4 || n._cutDirection == 5)
                    {
                        n._cutDirection = 0;
                    }
                    else if (n._cutDirection == 6 || n._cutDirection == 7)
                    {
                        n._cutDirection = 1;
                    }
                    if (notes[i + 1]._cutDirection == 4 || notes[i + 1]._cutDirection == 5)
                    {
                        notes[i + 1]._cutDirection = 0;
                    }
                    else if (notes[i + 1]._cutDirection == 6 || notes[i + 1]._cutDirection == 7)
                    {
                        notes[i + 1]._cutDirection = 1;
                    }
                }
            }

            //Fix same laneIndex hitbox issue
            for (int i = 0; i < notes.Count() - 1; i++)
            {
                Note n = notes[i];

                if (notes[i + 1]._time - n._time <= 0.02 && notes[i + 1]._time - n._time >= -0.02 && n._lineIndex == notes[i + 1]._lineIndex)
                {
                    if ((n._cutDirection == 0 || n._cutDirection == 1 || n._cutDirection == 4 || n._cutDirection == 5 || n._cutDirection == 6 || n._cutDirection == 7) && n._type == 0 && n._lineIndex != 0)
                    {
                        n._lineIndex--;
                    }
                    else if ((n._cutDirection == 0 || n._cutDirection == 1 || n._cutDirection == 4 || n._cutDirection == 5 || n._cutDirection == 6 || n._cutDirection == 7) && n._type == 1 && n._lineIndex != 3)
                    {
                        n._lineIndex++;
                    }
                    else if ((notes[i + 1]._cutDirection == 0 || notes[i + 1]._cutDirection == 1 || notes[i + 1]._cutDirection == 4 || notes[i + 1]._cutDirection == 5 || notes[i + 1]._cutDirection == 6 || notes[i + 1]._cutDirection == 7) && notes[i + 1]._type == 0 && notes[i + 1]._lineIndex != 0)
                    {
                        notes[i + 1]._lineIndex--;
                    }
                    else if ((notes[i + 1]._cutDirection == 0 || notes[i + 1]._cutDirection == 1 || notes[i + 1]._cutDirection == 4 || notes[i + 1]._cutDirection == 5 || notes[i + 1]._cutDirection == 6 || notes[i + 1]._cutDirection == 7) && notes[i + 1]._type == 1 && notes[i + 1]._lineIndex != 3)
                    {
                        notes[i + 1]._lineIndex++;
                    }
                }
            }

            //Fix shit-flow
            for (int i = 0; i < notes.Count() - 1; i++)
            {
                Note n = notes[i];

                if (lastBlue._cutDirection == 4 && n._cutDirection == 6 && n._type == 1)
                {
                    n._cutDirection = 1;
                }
                else if (lastBlue._cutDirection == 5 && n._cutDirection == 7 && n._type == 1)
                {
                    n._cutDirection = 1;
                }
                else if (lastBlue._cutDirection == 6 && n._cutDirection == 4 && n._type == 1)
                {
                    n._cutDirection = 0;
                }
                else if (lastBlue._cutDirection == 7 && n._cutDirection == 5 && n._type == 1)
                {
                    n._cutDirection = 0;
                }
                else if (lastRed._cutDirection == 5 && n._cutDirection == 7 && n._type == 0)
                {
                    n._cutDirection = 1;
                }
                else if (lastRed._cutDirection == 6 && n._cutDirection == 4 && n._type == 0)
                {
                    n._cutDirection = 0;
                }
                else if (lastRed._cutDirection == 7 && n._cutDirection == 5 && n._type == 0)
                {
                    n._cutDirection = 0;
                }
                else if (lastRed._cutDirection == 7 && n._cutDirection == 5 && n._type == 0)
                {
                    n._cutDirection = 0;
                }

                if ((n._cutDirection == 4 || n._cutDirection == 5) && n._lineIndex == 0)
                {
                    n._cutDirection = 0;
                }
                else if ((n._cutDirection == 4 || n._cutDirection == 5) && n._lineIndex == 3)
                {
                    n._cutDirection = 0;
                }
                else if ((n._cutDirection == 6 || n._cutDirection == 7) && n._lineIndex == 3)
                {
                    n._cutDirection = 1;
                }
                else if ((n._cutDirection == 6 || n._cutDirection == 7) && n._lineIndex == 0)
                {
                    n._cutDirection = 1;
                }

                if (lastRed._lineIndex == 0 && n._lineIndex > 0 && (n._cutDirection == 4 || n._cutDirection == 6))
                {
                    if (n._cutDirection == 4)
                    {
                        n._cutDirection = 0;
                    }
                    else if (n._cutDirection == 6)
                    {
                        n._cutDirection = 1;
                    }
                }

                if (lastBlue._lineIndex == 3 && n._lineIndex < 3 && (n._cutDirection == 5 || n._cutDirection == 7))
                {
                    if (n._cutDirection == 5)
                    {
                        n._cutDirection = 0;
                    }
                    else if (n._cutDirection == 7)
                    {
                        n._cutDirection = 1;
                    }
                }
                
                if(n._type == 0 && n._lineIndex >= 2)
                {
                    if (n._cutDirection == 4)
                    {
                        n._cutDirection = 0;
                    }
                    else if (n._cutDirection == 6)
                    {
                        n._cutDirection = 1;
                    }
                }
                else if (n._type == 1 && n._lineIndex <= 1)
                {
                    if (n._cutDirection == 5)
                    {
                        n._cutDirection = 0;
                    }
                    else if (n._cutDirection == 7)
                    {
                        n._cutDirection = 1;
                    }
                }

                if (n._type == 0)
                {
                    lastRed = notes[i];
                }
                else
                {
                    lastBlue = notes[i];
                }
            }
        }

        #endregion

        #region Process for patterns
        private void MapReader()
        {
            if(PatternToUse != "All" && PatternToUse != "Complex" && PatternToUse != "Random" && PatternToUse != "RandomStream")
            {
                PatternToUse = "All";
            }

            PatternCreator(PatternToUse);
        }

        void PatternCreator(string pattern)
        {
            double preceding = -1;
            int patternStart = -1;
            int available = 1;
            double duration = -1;
            int countFix = 0;
            Note before = new Note(-1, -1, -1, NoteType.Mine, (CutDirection)8);
            bool fixedPattern = false;
            List<Note> swapTime = new List<Note>();
            Note lastAddedNote = new Note(-1, -1, -1, NoteType.Mine, (CutDirection)8);

            if (pattern != "All" && pattern != "Random")
            {
                fixedPattern = true;
            }

            for (int i = 0; i < notes.Count - 2; i++)
            {
                double now = notes[i]._time;
                double next = notes[i + 1]._time;

                if (((now - preceding == duration || notes[i + 1]._time - notes[i]._time == duration || notes[i + 2]._time - notes[i + 1]._time == duration) && i != notes.Count - 3) || fixedPattern == true && i != notes.Count - 3) //Count the amount of notes available for the pattern, save the start location.
                {
                    available++;

                    if (patternStart == -1)
                    {
                        patternStart = i - 1;
                    }
                }
                else if(i != 0)
                {
                    if(i == notes.Count - 3)
                    {
                        available += 2;
                    }
                    if (patternStart == -1)
                    {
                        patternStart = i - 1;
                    }
                    if (duration <= 0.125 && fixedPattern == false)
                    {
                        pattern = "Vibro";
                    }
                    else if (duration <= 0.25 && fixedPattern == false)
                    {
                        pattern = "Stream";
                    }
                    else if (fixedPattern == false)
                    {
                        pattern = "Complex";
                    }

                    duration = 0;
                    int looper = countFix;

                    if(looper == -1)
                    {
                        looper = 0;
                    }

                    patternLoop = Pattern.GetNewPattern(pattern, 999);
                    if (pattern == "Stream")
                    {
                        patternLoop = new List<Note>(StreamPatternFix(patternLoop));
                    }
                    else if (pattern == "Vibro")
                    {
                        patternLoop = Pattern.GetNewPattern("Stream", 0);
                        patternLoop = new List<Note>(StreamPatternFix(patternLoop));
                    }
                    else if (pattern == "Random")
                    {
                        patternLoop = Pattern.GetNewPattern("Random", 999);
                    }
                    if (available < 7 && !fixedPattern)
                    {
                        if(patternStart == 0)
                        {
                            patternLoop = Pattern.GetNewPattern("Jump", 999);
                        }
                        else
                        {
                            do
                            {
                                patternLoop = Pattern.GetNewPattern("Jump", 999);
                            } while (patternLoop[looper]._lineIndex == notes[patternStart - 1]._lineIndex && patternLoop[looper]._lineLayer == notes[patternStart - 1]._lineLayer);
                        }
                    }
                    Note note = new Note(-1, -1, -1, NoteType.Mine, (CutDirection)8);

                    for (int j = 0; j < available; j++) 
                    {
                        note = notes[patternStart + j];

                        if (looper >= patternLoop.Count)
                        {
                            looper = countFix;

                            do
                            {
                                if (pattern == "Vibro")
                                {
                                    patternLoop = Pattern.GetNewPattern("Stream", 0);
                                    patternLoop = new List<Note>(StreamPatternFix(patternLoop));
                                    break;
                                }
                                else if (available - j < 7 && !fixedPattern)
                                {
                                    do
                                    {
                                        patternLoop = Pattern.GetNewPattern("Jump", 999);
                                    } while (patternLoop[looper]._lineIndex == notes[patternStart + j - 1]._lineIndex && patternLoop[looper]._lineLayer == notes[patternStart + j - 1]._lineLayer);
                                    break;
                                }
                                else if (pattern == "Complex" && 0 == RandNumber(0, 2))
                                {
                                    patternLoop = Pattern.GetNewPattern("Jump", 999);
                                }
                                else if (pattern == "Random")
                                {
                                    patternLoop = Pattern.GetNewPattern("Random", 999);
                                }
                                else if(pattern == "RandomStream")
                                {
                                    patternLoop = Pattern.GetNewPattern(pattern, 999);
                                    break;
                                }
                                else
                                {
                                    patternLoop = Pattern.GetNewPattern(pattern, 999);
                                    if (pattern == "Complex" && fixedPattern)
                                    {
                                        break;
                                    }
                                    if (pattern == "Stream")
                                    {
                                        patternLoop = new List<Note>(StreamPatternFix(patternLoop));
                                    }
                                }
                            } while (patternLoop.Count > available - j + 1);
                        }

                        note._lineIndex = patternLoop[looper]._lineIndex;
                        note._lineLayer = patternLoop[looper]._lineLayer;
                        note._cutDirection = patternLoop[looper]._cutDirection;
                        note._type = patternLoop[looper]._type;

                        notes[patternStart + j] = note;

                        countFix++;
                        looper++;
                        if (countFix == 4)
                        {
                            countFix = 0;
                        }

                        before = note;
                    }
                    available = 1;
                    patternStart = -1;
                    duration = next - now;
                }
                
                if(duration == -1)
                {
                    duration = next - now;
                }
                preceding = now;
            }

            notes = Notes.OrderBy(note => note._time).ToList();

            if(notes.Any() && notes.Count() > 3) // Fix last note
            {
                notes[notes.Count() - 1]._type = notes[notes.Count() - 3]._type;
                if(UpCut.Contains(notes[notes.Count() - 3]._cutDirection))
                {
                    notes[notes.Count() - 1]._cutDirection = 1;
                    notes[notes.Count() - 1]._lineIndex = notes[notes.Count() - 3]._lineIndex;
                    notes[notes.Count() - 1]._lineLayer = 0;
                }
                else if (DownCut.Contains(notes[notes.Count() - 3]._cutDirection))
                {
                    notes[notes.Count() - 1]._cutDirection = 0;
                    notes[notes.Count() - 1]._lineIndex = notes[notes.Count() - 3]._lineIndex;
                    notes[notes.Count() - 1]._lineLayer = 2;
                }
                else if (notes[notes.Count() - 3]._cutDirection == 2)
                {
                    notes[notes.Count() - 1]._cutDirection = 3;
                    notes[notes.Count() - 1]._lineIndex = 3;
                    notes[notes.Count() - 1]._lineLayer = notes[notes.Count() - 3]._lineLayer;
                }
                else if (notes[notes.Count() - 3]._cutDirection == 3)
                {
                    notes[notes.Count() - 1]._cutDirection = 2;
                    notes[notes.Count() - 1]._lineIndex = 0;
                    notes[notes.Count() - 1]._lineLayer = notes[notes.Count() - 3]._lineLayer;
                }
            }
        }

        List<Note> StreamPatternFix(List<Note> temp)
        {
            List<Note> newLoop = new List<Note>(temp);

            int id = Pattern.GetPatternID();
            if(id == 0) //normy stream
            {
                int line = RandNumber(1, 4);
                newLoop[0]._lineIndex = line;
                if (line == 3)
                {
                    newLoop[1]._lineIndex = 2;
                    newLoop[2]._lineIndex = 3;
                    newLoop[3]._lineIndex = 2;
                }
                else if (line == 2)
                {
                    newLoop[1]._lineIndex = 1;
                    newLoop[2]._lineIndex = 2;
                    newLoop[3]._lineIndex = 1;
                }
                else
                {
                    newLoop[1]._lineIndex = 0;
                    newLoop[2]._lineIndex = 1;
                    newLoop[3]._lineIndex = 0;
                }
            }
            else if (id == 10) //one-lane
            {
                int line = RandNumber(0, 4);
                newLoop[0]._lineIndex = line;
                newLoop[1]._lineIndex = line;
                newLoop[2]._lineIndex = line;
                newLoop[3]._lineIndex = line;
            }
            else if(id == 4) //banana
            {
                for(int i = 0; i < 8; i++)
                {
                    if (newLoop[i]._cutDirection == 1 && newLoop[i]._type == 1 && newLoop[i]._lineIndex == 3)
                    {
                        newLoop[i]._cutDirection = 7;
                    }
                    else if (newLoop[i]._cutDirection == 1 && newLoop[i]._type == 1 && newLoop[i]._lineIndex == 0)
                    {
                        newLoop[i]._cutDirection = 6;
                    }
                }
            }
            else if(id == 14) //inward to outward
            {
                if(RandNumber(0, 2) == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (newLoop[i]._cutDirection == 0 && newLoop[i]._type == 1 && newLoop[i]._lineLayer == 1)
                        {
                            newLoop[i]._cutDirection = 5;
                        }
                        else if (newLoop[i]._cutDirection == 0 && newLoop[i]._type == 0 && newLoop[i]._lineLayer == 1)
                        {
                            newLoop[i]._cutDirection = 4;
                        }
                    }
                }
            }

            return newLoop;
        }

        #endregion

        #region Process for map modification

        void BottomDisplacement()
        {
            foreach (var note in Notes)
            {
                if(note._lineLayer == 2 || note._lineLayer == 1)
                {
                    note._lineLayer = 0;
                }
            }
        }

        void AllUpTop()
        {
            foreach (var note in Notes)
            {
                if ((note._lineLayer == 0 || note._lineLayer == 1) && (note._cutDirection == (int)CutDirection.Up || note._cutDirection == (int)CutDirection.UpLeft || note._cutDirection == (int)CutDirection.UpRight))
                {
                    note._lineLayer = 2;
                }
            }
        }

        void UpDown()
        {
            foreach (var note in Notes)
            {
                if(note._cutDirection == 6 || note._cutDirection == 7)
                {
                    note._cutDirection = 1;
                }
                else if (note._cutDirection == 4 || note._cutDirection == 5)
                {
                    note._cutDirection = 0;
                }
                else if(note._cutDirection == 2)
                {
                    note._cutDirection = 1;
                }
                else if (note._cutDirection == 3)
                {
                    note._cutDirection = 1;
                }
            }
        }

        #endregion
    }
}
