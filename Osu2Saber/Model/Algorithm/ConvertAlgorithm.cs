
using Newtonsoft.Json;
using Osu2Saber.Model.Json;
using osuBMParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Osu2Saber.Model.Algorithm
{
    public class ConvertAlgorithm
    {
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
        public static bool UseLogic = false;
        public static bool AllowOneHanded = false;

        protected const float OsuScreenXMax = 512, OsuScreenYMax = 384;

        public static double SlowSpeed = 0.4;
        public static double EnoughIntervalBetweenNotes = 0.2;
        public static double GallopSpeed = 0.3;
        public static string PatternToUse = "Pack";

        protected Beatmap org;
        protected SaberBeatmap dst;

        protected List<Event> events = new List<Event>();
        protected List<Obstacle> obstacles = new List<Obstacle>();
        protected List<Note> notes = new List<Note>();
        protected double bpm;
        protected double realBpm;
        protected List<double> bpmPerNote = new List<double>();
        protected int offset;
        protected bool isMania = false;
        protected Event ev;
        protected int forceSpread = 0;
        protected int first = 0;

        public List<_Pattern> patterns = new List<_Pattern>();
        public List<_Pattern> slowPatterns = new List<_Pattern>();
        public List<Event> Events => events;
        public List<Obstacle> Obstacles => obstacles;
        public List<Note> Notes => notes;

        public List<int> UpCut = new List<int>() { 0, 4, 5 };
        public List<int> DownCut = new List<int>() { 1, 6, 7 };
        public List<int> IntoLeft = new List<int>() { 3, 5, 7 };
        public List<int> IntoRight = new List<int>() { 2, 4, 6 };

        public ConvertAlgorithm(Beatmap osu, SaberBeatmap bs)
        {
            org = osu;
            dst = bs;
            bpm = dst._beatsPerMinute;
            realBpm = bpm;
            offset = osu.TimingPoints[0].Offset;
            isMania = org.Mode == 3;
        }

        public ConvertAlgorithm(List<Note> note)
        {
            Notes.AddRange(note);
            var pd = PromptDialog.Prompt("Enter the BPM", "BPM");
            if (pd != null)
            {
                bpm = double.Parse(pd);
                realBpm = double.Parse(pd);
                foreach (var n in notes)
                {
                    bpmPerNote.Add(double.Parse(pd));
                }
            }
        }

        // You may override this method for better map generation
        public void Convert()
        {
            MakeHitObjects();
            if (!OnlyMakeTimingNote)
            {
                RemoveExcessNotes();
                MapReader();
                if(UseLogic)
                {
                    LogicChecker();
                }
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

        public void ConvertDat()
        {
            RemoveExcessNotes();
            MapReader();
            if (UseLogic)
            {
                LogicChecker();
            }
            if (AllTopUp)
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
                if(obj is HoldNote)
                {
                    var temp = (HoldNote)obj;

                    AddNote(temp.Time, temp.Position.x, temp.Position.y);
                }
                else if (obj is HitSlider)
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

            // Handle variable BPM
            for (int i = org.TimingPoints.Count() - 1; i > -1; i--)
            {
                if (org.TimingPoints[i].Offset <= timeMs && org.TimingPoints[i].MsPerBeat > 5 && org.TimingPoints[i].MsPerBeat < 1000)
                {
                    realBpm = Math.Round(1000.0 / org.TimingPoints[i].MsPerBeat * 60);
                    break;
                }
            }

            var time = ConvertTime(timeMs);
            var note = new Note(time, line, layer, 0, CutDirection.Any);
            notes.Add(note);
            bpmPerNote.Add(realBpm);
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
                    if (notes[i]._time - notes[i - 1]._time >= -0.01 && notes[i]._time - notes[i - 1]._time <= 0.01 && notes[i - 1]._time - notes[i - 2]._time <= GallopSpeed * (bpm / bpmPerNote[i]))
                    {
                        notes.Remove(notes[i]);
                    }
                }
                for (int i = notes.Count() - 2; i > 0; i--)
                {
                    if (notes[i]._time - notes[i - 1]._time >= -0.01 && notes[i]._time - notes[i - 1]._time <= 0.01 && notes[i + 1]._time - notes[i]._time <= GallopSpeed * (bpm / bpmPerNote[i]))
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
            int i = 0;

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
                else if(bpm != bpmPerNote[i])
                {
                    if (timeGap >= (EnoughIntervalBetweenNotes * (bpm / bpmPerNote[i])))
                    {
                        keepNotes.Add(note);
                        lastTime = note._time;
                        dbl = false;
                    }
                }
                else if (timeGap >= EnoughIntervalBetweenNotes)
                {
                    keepNotes.Add(note);
                    lastTime = note._time;
                    dbl = false;
                }

                i++;
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
            return Math.Round(sectionIdx / 8.0, 3, MidpointRounding.AwayFromZero) + 0.125;
        }

        protected int ConvertBeat(double timeBeat, double bpm)
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

                if (notes[i + 1]._time - n._time <= 0.02 && notes[i + 1]._time - n._time >= -0.02 && n._lineLayer == notes[i + 1]._lineLayer && n._lineIndex == notes[i + 1]._lineIndex)
                {
                    if (n._type == 0 && n._lineIndex <= 1)
                    {
                        notes[i + 1]._lineIndex = n._lineIndex + 1;
                    }
                    else if (n._type == 1 && n._lineIndex >= 2)
                    {
                        notes[i + 1]._lineIndex = n._lineIndex - 1;
                    }
                    else if (n._type == 1 && notes[i + 1]._lineIndex <= 1)
                    {
                        n._lineIndex = notes[i + 1]._lineIndex + 1;
                    }
                    else if (n._type == 0 && notes[i + 1]._lineIndex >= 2)
                    {
                        n._lineIndex = notes[i + 1]._lineIndex - 1;
                    }
                }
            }

            //Fix hitbox issue
            for (int i = 0; i < notes.Count() - 2; i++)
            {
                Note now = notes[i];
                Note next = notes[i + 1];

                if (next._time - now._time <= 0.02 && next._time - now._time >= -0.02)
                {
                    if ((now._cutDirection == 0 || now._cutDirection == 1) && now._lineIndex == next._lineIndex)
                    {
                        if (next._lineIndex != 3 && next._type == 1)
                        {
                            next._lineIndex++;
                            if (next._lineLayer == 1 && (next._lineIndex == 1 || next._lineIndex == 2))
                            {
                                if (next._cutDirection == 0)
                                {
                                    next._lineLayer = 2;
                                }
                                else
                                {
                                    next._lineLayer = 0;
                                }
                            }
                        }
                        else if (now._lineIndex != 3 && now._type == 1)
                        {
                            now._lineIndex++;
                            if (now._lineLayer == 1 && (now._lineIndex == 1 || now._lineIndex == 2))
                            {
                                if (now._cutDirection == 0)
                                {
                                    now._lineLayer = 2;
                                }
                                else
                                {
                                    now._lineLayer = 0;
                                }
                            }
                        }
                        else if (now._lineIndex != 0 && now._type == 0)
                        {
                            now._lineIndex--;
                            if (now._lineLayer == 1 && (now._lineIndex == 1 || now._lineIndex == 2))
                            {
                                if (now._cutDirection == 0)
                                {
                                    now._lineLayer = 2;
                                }
                                else
                                {
                                    now._lineLayer = 0;
                                }
                            }
                        }
                    }
                    else if((next._cutDirection == 0 || next._cutDirection == 1) && now._lineIndex == next._lineIndex)
                    {
                        if (next._lineIndex != 3 && next._type == 1)
                        {
                            next._lineIndex++;
                            if (next._lineLayer == 1 && (next._lineIndex == 1 || next._lineIndex == 2))
                            {
                                if (next._cutDirection == 0)
                                {
                                    next._lineLayer = 2;
                                }
                                else
                                {
                                    next._lineLayer = 0;
                                }
                            }
                        }
                        else if(now._lineIndex != 3 && now._type == 1)
                        {
                            now._lineIndex++;
                            if (now._lineLayer == 1 && (now._lineIndex == 1 || now._lineIndex == 2))
                            {
                                if (now._cutDirection == 0)
                                {
                                    now._lineLayer = 2;
                                }
                                else
                                {
                                    now._lineLayer = 0;
                                }
                            }
                        }
                        else if (now._lineIndex != 0 && now._type == 0)
                        {
                            now._lineIndex--;
                            if (now._lineLayer == 1 && (now._lineIndex == 1 || now._lineIndex == 2))
                            {
                                if (now._cutDirection == 0)
                                {
                                    now._lineLayer = 2;
                                }
                                else
                                {
                                    now._lineLayer = 0;
                                }
                            }
                        }
                    }
                    else if ((now._cutDirection == 2 || now._cutDirection == 3) && next._lineLayer == now._lineLayer)
                    {
                        if ((now._lineIndex == 0 || now._lineIndex == 3) && now._lineLayer != 0)
                        {
                            now._lineLayer--;
                        }
                        else if ((now._lineIndex == 0 || now._lineIndex == 3) && now._lineLayer != 3)
                        {
                            now._lineLayer++;
                        }
                        else if ((next._lineIndex == 0 || next._lineIndex == 3) && next._lineLayer != 0)
                        {
                            next._lineLayer--;
                        }
                        else if ((next._lineIndex == 0 || next._lineIndex == 3) && next._lineLayer != 3)
                        {
                            next._lineLayer++;
                        }
                    }
                    if ((now._cutDirection == 4 || now._cutDirection == 7) && ((next._lineIndex == now._lineIndex - 1 && next._lineLayer == now._lineLayer + 1) || (next._lineIndex == now._lineIndex + 1 && next._lineLayer == now._lineLayer - 1) || (next._lineIndex == now._lineIndex && next._lineLayer == now._lineLayer + 1) || (next._lineIndex == now._lineIndex && next._lineLayer == now._lineLayer - 1) || (next._lineIndex == now._lineIndex - 1 && next._lineLayer == now._lineLayer) || (next._lineIndex == now._lineIndex + 1 && next._lineLayer == now._lineLayer)))
                    {
                        if (next._cutDirection > 3)
                        {
                            if (now._lineLayer == 2)
                            {
                                now._lineLayer = 0;
                                now._lineIndex = next._lineIndex;
                            }
                            else
                            {
                                now._lineLayer = 2;
                                now._lineIndex = next._lineIndex;
                            }
                        }
                        else
                        {
                            if (now._lineLayer == 2)
                            {
                                now._lineLayer = 0;
                            }
                            else
                            {
                                now._lineLayer = 2;
                            }
                        }
                    }
                    if ((now._cutDirection == 5 || now._cutDirection == 6) && ((next._lineIndex == now._lineIndex + 1 && next._lineLayer == now._lineLayer + 1) || (next._lineIndex == now._lineIndex - 1 && next._lineLayer == now._lineLayer - 1) || (next._lineIndex == now._lineIndex && next._lineLayer == now._lineLayer + 1) || (next._lineIndex == now._lineIndex && next._lineLayer == now._lineLayer - 1) || (next._lineIndex == now._lineIndex - 1 && next._lineLayer == now._lineLayer) || (next._lineIndex == now._lineIndex + 1 && next._lineLayer == now._lineLayer)))
                    {
                        if (next._cutDirection > 3)
                        {
                            if (now._lineLayer == 2)
                            {
                                now._lineLayer = 0;
                                now._lineIndex = next._lineIndex;
                            }
                            else
                            {
                                now._lineLayer = 2;
                                now._lineIndex = next._lineIndex;
                            }
                        }
                        else
                        {
                            if (now._lineLayer == 2)
                            {
                                now._lineLayer = 0;
                            }
                            else
                            {
                                now._lineLayer = 2;
                            }
                        }
                    }
                    if ((next._cutDirection == 4 || next._cutDirection == 7) && ((now._lineIndex == next._lineIndex - 1 && now._lineLayer == next._lineLayer + 1) || (now._lineIndex == next._lineIndex + 1 && now._lineLayer == next._lineLayer - 1) || (now._lineIndex == next._lineIndex && now._lineLayer == next._lineLayer + 1) || (now._lineIndex == next._lineIndex && now._lineLayer == next._lineLayer - 1) || (now._lineIndex == next._lineIndex - 1 && now._lineLayer == next._lineLayer) || (now._lineIndex == next._lineIndex + 1 && now._lineLayer == next._lineLayer)))
                    {
                        if (now._cutDirection > 3)
                        {
                            if (next._lineLayer == 2)
                            {
                                next._lineLayer = 0;
                                next._lineIndex = now._lineIndex;
                            }
                            else
                            {
                                next._lineLayer = 2;
                                next._lineIndex = now._lineIndex;
                            }
                        }
                        else
                        {
                            if (next._lineLayer == 2)
                            {
                                next._lineLayer = 0;
                            }
                            else
                            {
                                next._lineLayer = 2;
                            }
                        }
                    }
                    if ((next._cutDirection == 5 || next._cutDirection == 6) && ((now._lineIndex == next._lineIndex + 1 && now._lineLayer == next._lineLayer + 1) || (now._lineIndex == next._lineIndex - 1 && now._lineLayer == next._lineLayer - 1) || (now._lineIndex == next._lineIndex && now._lineLayer == next._lineLayer + 1) || (now._lineIndex == next._lineIndex && now._lineLayer == next._lineLayer - 1) || (now._lineIndex == next._lineIndex - 1 && now._lineLayer == next._lineLayer) || (now._lineIndex == next._lineIndex + 1 && now._lineLayer == next._lineLayer)))
                    {
                        if (now._cutDirection > 3)
                        {
                            if (next._lineLayer == 2)
                            {
                                next._lineLayer = 0;
                                next._lineIndex = now._lineIndex;
                            }
                            else
                            {
                                next._lineLayer = 2;
                                next._lineIndex = now._lineIndex;
                            }
                        }
                        else
                        {
                            if (next._lineLayer == 2)
                            {
                                next._lineLayer = 0;
                            }
                            else
                            {
                                next._lineLayer = 2;
                            }
                        }
                    }
                    if(now._lineIndex == next._lineIndex && (now._lineLayer == next._lineLayer + 1 || now._lineLayer == next._lineLayer -1))
                    {
                        if(now._cutDirection != 2 && now._cutDirection != 3 && next._cutDirection != 2 && next._cutDirection != 3)
                        {
                            if (now._lineLayer == 0)
                            {
                                next._lineLayer = 2;
                            }
                            else if (now._lineLayer == 1 && next._lineLayer == 0)
                            {
                                now._lineLayer = 2;
                            }
                            else if (now._lineLayer == 1 && next._lineLayer == 2)
                            {
                                now._lineLayer = 0;
                            }
                            else if(now._lineLayer == 2)
                            {
                                next._lineLayer = 0;
                            }
                        }
                    }
                }
            }

            //Flow fix
            for (int i = 0; i < notes.Count() - 2; i++)
            {
                Note now = notes[i];
                Note other = notes[i + 1];
                Note next = notes[i + 2];

                if (now._cutDirection == 5 && next._cutDirection == 7 && now._type == 1)
                {
                    next._cutDirection = 1;
                }
                else if (now._cutDirection == 4 && next._cutDirection == 6 && now._type == 0)
                {
                    next._cutDirection = 1;
                }

                if(now._cutDirection == 4 && now._type == 0 && now._lineIndex > 1)
                {
                    now._cutDirection = 0;
                }
                else if (now._cutDirection == 5 && now._type == 1 && now._lineIndex < 2)
                {
                    now._cutDirection = 0;
                }
                else if (now._cutDirection == 6 && now._type == 0 && now._lineIndex > 1)
                {
                    now._cutDirection = 1;
                }
                else if (now._cutDirection == 7 && now._type == 1 && now._lineIndex < 2)
                {
                    now._cutDirection = 1;
                }

                if(now._cutDirection == 2 && next._cutDirection != 7)
                {
                    next._cutDirection = 7;
                    if(next._time - other._time <= 0.01 && next._time - other._time >= -0.01 && other._lineIndex == next._lineIndex + 1 && other._lineLayer == 0)
                    {
                        other._lineLayer++;
                    }
                }
                else if (now._cutDirection == 3 && next._cutDirection != 6)
                {
                    next._cutDirection = 6;
                    if (next._time - other._time <= 0.01 && next._time - other._time >= -0.01 && other._lineIndex == next._lineIndex - 1 && other._lineLayer == 0)
                    {
                        other._lineLayer++;
                    }
                }

                if(now._cutDirection == 0 && next._cutDirection == 6 && now._type == 1 && next._lineIndex > now._lineIndex)
                {
                    next._lineIndex--;
                }
                else if (now._cutDirection == 0 && next._cutDirection == 7 && now._type == 0 && next._lineIndex < now._lineIndex)
                {
                    next._lineIndex++;
                }
            }
        }

        #endregion

        #region Process for patterns
        private void MapReader()
        {
            if(PatternToUse != "Pack" && PatternToUse != "RandomStream" && PatternToUse != "Complex")
            {
                PatternToUse = "Pack";
            }
            if(PatternToUse == "Pack")
            {
                MessageBox.Show("Select the pack to use for faster part.");
                OpenFileDialog open = new OpenFileDialog
                {
                    Filter = "pak|*.pak",
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    Multiselect = false
                };

                if (open.ShowDialog() == true)
                {
                    string data = File.ReadAllText(open.FileName);
                    Pack pack = JsonConvert.DeserializeObject<Pack>(data);
                    patterns.AddRange(pack._pattern);
                }
                else
                {
                    MessageBox.Show("Error during Pack selection, closing.");
                    Process.GetCurrentProcess().Kill();
                }

                MessageBox.Show("Select the pack to use for slower part.");
                open = new OpenFileDialog
                {
                    Filter = "pak|*.pak",
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    Multiselect = false
                };

                if (open.ShowDialog() == true)
                {
                    string data = File.ReadAllText(open.FileName);
                    Pack pack = JsonConvert.DeserializeObject<Pack>(data);
                    slowPatterns.AddRange(pack._pattern);
                }
                else
                {
                    MessageBox.Show("Error during Pack selection, closing.");
                    Process.GetCurrentProcess().Kill();
                }
            }

            PatternCreator(PatternToUse);
        }

        void PatternCreator(string pattern)
        {
            int available = notes.Count;
            // Current slow section
            bool slow = false;
            // To find the right pattern
            bool foundBlue = false;
            bool foundRed = false;
            // To know the placement order of notes.
            List<int> noteOrder = new List<int>();
            // Note to be used from pattern
            Queue<Note> blueNote = new Queue<Note>();
            Queue<Note> redNote = new Queue<Note>();
            // To convert to queue
            List<Note> patternList = new List<Note>();
            // Current direction
            int leftHand = 0;
            int rightHand = 0;
            // Last note used
            Note preceding = new Note(-1, -1, -1, -1, -1);

            Note n = new Note(-1, -1, -1, NoteType.Mine, (CutDirection)8);

            //For each notes
            for (int i = 0; i < available - 2; i++)
            {
                n = notes[i];

                // If the pattern is done or the speed got faster than slow (ignore double).
                if (noteOrder.Count() == 0 || (slow && notes[i + 1]._time - notes[i]._time < SlowSpeed * (bpm / bpmPerNote[i]) && notes[i + 1]._time - notes[i]._time >= 0.01))
                {
                    // Infinite loop until a pattern that fit the condition is met.
                    do
                    {
                        // Need to find a new pattern.
                        foundBlue = false;
                        foundRed = false;
                        slow = false;

                        // Clear/create a new loop.
                        blueNote.Clear();
                        redNote.Clear();
                        noteOrder.Clear();

                        // Slow speed
                        if (notes[i + 1]._time - notes[i]._time >= SlowSpeed * (bpm / bpmPerNote[i]) || (notes[i + 1]._time - notes[i]._time > -0.01 && notes[i + 1]._time - notes[i]._time <= 0.01 && notes[i + 2]._time - notes[i + 1]._time >= SlowSpeed * (bpm / bpmPerNote[i])))
                        {
                            if (PatternToUse == "Pack")
                            {
                                foreach (var no in slowPatterns.ElementAt(RandNumber(0, slowPatterns.Count()))._notes)
                                {
                                    Note not = new Note(no);
                                    if(not._type == 0)
                                    {
                                        noteOrder.Add(0);
                                        redNote.Enqueue(not);
                                    }
                                    else if(not._type == 1)
                                    {
                                        noteOrder.Add(1);
                                        blueNote.Enqueue(not);
                                    }
                                }
                            }
                            else
                            {
                                patternList = Pattern.GetNewPattern(pattern, 999);
                                foreach(var no in patternList)
                                {
                                    Note not = new Note(no);
                                    if (not._type == 0)
                                    {
                                        noteOrder.Add(0);
                                        redNote.Enqueue(not);
                                    }
                                    else if (not._type == 1)
                                    {
                                        noteOrder.Add(1);
                                        blueNote.Enqueue(not);
                                    }
                                }
                            }
                            slow = true;
                        }
                        else // Fast speed
                        {
                            if (PatternToUse == "Pack")
                            {
                                foreach (var no in patterns.ElementAt(RandNumber(0, patterns.Count()))._notes)
                                {
                                    Note not = new Note(no);
                                    if (not._type == 0)
                                    {
                                        noteOrder.Add(0);
                                        redNote.Enqueue(not);
                                    }
                                    else if (not._type == 1)
                                    {
                                        noteOrder.Add(1);
                                        blueNote.Enqueue(not);
                                    }
                                }
                            }
                            else
                            {
                                patternList = Pattern.GetNewPattern(pattern, 999);
                                foreach (var no in patternList)
                                {
                                    Note not = new Note(no);
                                    if (not._type == 0)
                                    {
                                        noteOrder.Add(0);
                                        redNote.Enqueue(not);
                                    }
                                    else if (not._type == 1)
                                    {
                                        noteOrder.Add(1);
                                        blueNote.Enqueue(not);
                                    }
                                }
                            }
                        }

                        // Check if condition is met
                        for (var blue = 0; blue < blueNote.Count(); blue++)
                        {
                            if(DownCut.Contains(rightHand) && UpCut.Contains(blueNote.Peek()._cutDirection))
                            {
                                foundBlue = true;
                                break;
                            }
                            else if(UpCut.Contains(rightHand) && DownCut.Contains(blueNote.Peek()._cutDirection))
                            {
                                foundBlue = true;
                                break;
                            }
                            else if(rightHand == 2 && IntoLeft.Contains(blueNote.Peek()._cutDirection))
                            {
                                foundBlue = true;
                                break;
                            }
                            else if(rightHand == 3 && IntoRight.Contains(blueNote.Peek()._cutDirection))
                            {
                                foundBlue = true;
                                break;
                            }
                            else if(IntoLeft.Contains(rightHand) && blueNote.Peek()._cutDirection == 2)
                            {
                                foundBlue = true;
                                break;
                            }
                            else if(IntoRight.Contains(rightHand) && blueNote.Peek()._cutDirection == 3)
                            {
                                foundBlue = true;
                                break;
                            }
                            blueNote.Enqueue(blueNote.Dequeue());
                        }

                        for (var red = 0; red < redNote.Count(); red++)
                        {
                            if (DownCut.Contains(leftHand) && UpCut.Contains(redNote.Peek()._cutDirection))
                            {
                                foundRed = true;
                                break;
                            }
                            else if (UpCut.Contains(leftHand) && DownCut.Contains(redNote.Peek()._cutDirection))
                            {
                                foundRed = true;
                                break;
                            }
                            else if (leftHand == 2 && IntoLeft.Contains(redNote.Peek()._cutDirection))
                            {
                                foundRed = true;
                                break;
                            }
                            else if (leftHand == 3 && IntoRight.Contains(redNote.Peek()._cutDirection))
                            {
                                foundRed = true;
                                break;
                            }
                            else if (IntoLeft.Contains(leftHand) && redNote.Peek()._cutDirection == 2)
                            {
                                foundRed = true;
                                break;
                            }
                            else if (IntoRight.Contains(leftHand) && redNote.Peek()._cutDirection == 3)
                            {
                                foundRed = true;
                                break;
                            }
                            redNote.Enqueue(redNote.Dequeue());
                        }

                        if((blueNote.Count() == 0 && redNote.Count() != 0) || (redNote.Count() == 0 && blueNote.Count != 0) && AllowOneHanded)
                        {
                            break;
                        }
                    } while (!foundBlue || !foundRed);
                }

                if (i != 0) // Check for double issue
                {
                    if (notes[i]._time - notes[i - 1]._time <= 0.01 && notes[i]._time - notes[i - 1]._time >= -0.01) // This will be a double
                    {
                        if (notes[i - 1]._type == 0 && noteOrder.First() == 0) // Both are going to be red, we don't want that.
                        {
                            if(noteOrder.Any(x => x == 1)) // There's blue left in the list
                            {
                                noteOrder.Remove(1);
                                noteOrder.Insert(0, 1);
                            }
                            else // No more blue, we need to get a new pattern.
                            {
                                noteOrder.Clear();
                                i--;
                                continue;
                            }
                        }
                        else if(notes[i - 1]._type == 1 && noteOrder.First() == 1) // Both are going to be blue, we don't want that.
                        {
                            if (noteOrder.Any(x => x == 0)) // There's red left in the list
                            {
                                noteOrder.Remove(0);
                                noteOrder.Insert(0, 0);
                            }
                            else // No more red, we need to get a new pattern.
                            {
                                noteOrder.Clear();
                                i--;
                                continue;
                            }
                        }
                    }
                }

                // Get the current note type
                int noteType = noteOrder.First();
                noteOrder.Remove(noteOrder.First());

                // Create the note
                if(noteType == 0)
                {
                    Note note = redNote.Dequeue();
                    n._lineIndex = note._lineIndex;
                    n._lineLayer = note._lineLayer;
                    n._cutDirection = note._cutDirection;
                    n._type = note._type;
                }
                else if (noteType == 1)
                {
                    Note note = blueNote.Dequeue();
                    n._lineIndex = note._lineIndex;
                    n._lineLayer = note._lineLayer;
                    n._cutDirection = note._cutDirection;
                    n._type = note._type;
                }

                // Check for fused notes issue
                if(preceding._time - n._time >= -0.01 && preceding._time - n._time <= 0.01)
                {
                    if(preceding._lineIndex == n._lineIndex && preceding._lineLayer == n._lineLayer)
                    {
                        if(n._type == 0 && n._lineIndex != 0)
                        {
                            n._lineIndex--;
                        }
                        else if(n._type == 1 && n._lineIndex != 3)
                        {
                            n._lineIndex++;
                        }
                        else if (preceding._type == 0 && preceding._lineIndex != 0)
                        {
                            preceding._lineIndex--;
                        }
                        else if (preceding._type == 1 && preceding._lineIndex != 3)
                        {
                            preceding._lineIndex++;
                        }
                    }
                }

                // Add the note
                notes[i] = n;

                // Follow up the flow per hand
                if (n._type == 0)
                {
                    leftHand = n._cutDirection;
                }
                else if(n._type == 1)
                {
                    rightHand = n._cutDirection;
                }

                preceding = notes[i];
            }

            // Set notes by time order.
            notes = Notes.OrderBy(note => note._time).ToList();

            if (notes.Any() && notes.Count() > 3) // Fix the last 2 notes
            {
                notes[notes.Count() - 1]._type = notes[notes.Count() - 3]._type;
                if (UpCut.Contains(notes[notes.Count() - 3]._cutDirection))
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

                notes[notes.Count() - 2]._type = notes[notes.Count() - 4]._type;
                if (UpCut.Contains(notes[notes.Count() - 4]._cutDirection))
                {
                    notes[notes.Count() - 2]._cutDirection = 1;
                    notes[notes.Count() - 2]._lineIndex = notes[notes.Count() - 4]._lineIndex;
                    notes[notes.Count() - 2]._lineLayer = 0;
                }
                else if (DownCut.Contains(notes[notes.Count() - 4]._cutDirection))
                {
                    notes[notes.Count() - 2]._cutDirection = 0;
                    notes[notes.Count() - 2]._lineIndex = notes[notes.Count() - 4]._lineIndex;
                    notes[notes.Count() - 2]._lineLayer = 2;
                }
                else if (notes[notes.Count() - 4]._cutDirection == 2)
                {
                    notes[notes.Count() - 2]._cutDirection = 3;
                    notes[notes.Count() - 2]._lineIndex = 3;
                    notes[notes.Count() - 2]._lineLayer = notes[notes.Count() - 4]._lineLayer;
                }
                else if (notes[notes.Count() - 4]._cutDirection == 3)
                {
                    notes[notes.Count() - 2]._cutDirection = 2;
                    notes[notes.Count() - 2]._lineIndex = 0;
                    notes[notes.Count() - 2]._lineLayer = notes[notes.Count() - 4]._lineLayer;
                }
            }
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
                    note._cutDirection = 0;
                }
            }
        }

        #endregion
    }
}
