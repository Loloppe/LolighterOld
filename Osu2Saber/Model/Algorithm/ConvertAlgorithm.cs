using Osu2Saber.Model.Json;
using osuBMParser;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;

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

        int NegligibleTimeDiffMs = 500; //is used for the first note and event time gap
        int EnoughIntervalMs = 1500;  //is used to determine whether to reset cut direction
        int EnoughIntervalForSymMs = 800; //is used to determine to set a symmetric note
        float EnoughInterval = 3f;  //is used to determine whether to reset cut direction
        float EnoughIntervalForSym = 2f; //is used to determine to set a symmetric note
        public static float EnoughIntervalBetweenNotes = 0.250f;
        public static int Mix = 2;
        public static double LightOffset = 0.0D;
        public static double LimitStacked = 2;
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
        

        public ConvertAlgorithm(Beatmap osu, SaberBeatmap bs)
        {
            org = osu;
            dst = bs;
            bpm = dst._beatsPerMinute;
            offset = osu.TimingPoints[0].Offset;
            isMania = org.Mode == 3;

            EnoughIntervalMs = Math.Min(EnoughIntervalMs, ConvertBeat(EnoughInterval));
            EnoughIntervalForSym = Math.Min(EnoughIntervalForSym, ConvertBeat(EnoughInterval));
        }

        // You may override this method for better map generation
        public void Convert()
        {
            MakeHitObjects();
            if (!OnlyMakeTimingNote)
            {
                RemoveExcessNotes();
                SetCutDirection();
                AddSymmetryNotes();
                MapReader();
                LogicChecker();
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
                if (obj.Time < NegligibleTimeDiffMs) continue;
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
            var note = new Note(ConvertTime(timeMs), line, layer, DetermineColor(line, layer), CutDirection.Any);
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
            if (DetermineColor(line, 0) == NoteType.Red)
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

        NoteType DetermineColor(int line, int layer)
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
            int count = 0;

            var rightNotes = Notes.Where(note => note._type == (int)(NoteType.Blue)).ToList();
            var leftNotes = Notes.Where(note => note._type == (int)(NoteType.Red)).ToList();

            var newRight = RemoveExcessNotes(rightNotes);
            var newLeft = RemoveExcessNotes(leftNotes);

            foreach (var note in newLeft)
            {
                newRight.Add(note);
            }

            newRight = newRight.OrderBy(note => note._time).ToList();

            if (CreateDouble && !GenerateGallops) // Stream + Double
            {
                for (int i = newRight.Count() - 1; i > 2; i--)
                {
                    if (newRight[i]._time - newRight[i - 1]._time >= -0.01 && newRight[i]._time - newRight[i - 1]._time <= 0.01 && newRight[i - 2]._time - newRight[i - 3]._time >= 0.1)
                    {
                        // Gallops
                        if (newRight[i - 2]._type == newRight[i - 1]._type)
                        {
                            newRight.Remove(newRight[i - 1]);
                        }
                        else if (newRight[i - 2]._type == newRight[i]._type)
                        {
                            newRight.Remove(newRight[i]);
                        }
                    }
                }
            }
            else if (!CreateDouble) // Stream
            {
                for (int i = newRight.Count() - 1; i > 0; i--)
                {
                    if (newRight[i]._time - newRight[i - 1]._time >= -0.01 && newRight[i]._time - newRight[i - 1]._time <= 0.01)
                    {
                        count++;
                        Console.Write(count.ToString());
                        newRight.Remove(newRight[i]); // Doesn't matter between i or i - 1 since the Automapper will overwrite it anyway.
                    }
                }
            }

            notes = newRight.OrderBy(note => note._time).ToList();
        }

        List<Note> RemoveExcessNotes(List<Note> notes)
        {
            double lastTime = -100;
            var newNotes = new List<Note>();
            var rushCount = 0;
            Note savedNote = null;

            foreach (var note in notes)
            {
                var timeGap = note._time - lastTime;
                if (timeGap >= EnoughIntervalBetweenNotes)
                {
                    // the timeGap is long enough
                    rushCount = 0;

                    newNotes.Add(note);

                    lastTime = note._time;
                }
                else
                {
                    if (savedNote == null)
                    {
                        savedNote = note;
                        rushCount = 0;
                    }
                    rushCount++;
                }
            }
            return newNotes;
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

        #region Process for cut direction
        void SetCutDirection()
        {
            if (NoDirectionAndPlacement) return;
            var n = Notes.Count;
            if (n == 0) return;
            var rightNotes = Notes.Where(note => note._type == (int)(NoteType.Blue)).ToList();
            var leftNotes = Notes.Where(note => note._type == (int)(NoteType.Red)).ToList();

            SetCutDirection(rightNotes);
            SetCutDirection(leftNotes);
        }

        void SetCutDirection(List<Note> notes)
        {
            var n = notes.Count;
            if (n < 1) return;

            // the first cut direction can be determined independently
            notes[0]._cutDirection = (int)PickBestDirectionSingle(notes[0]._lineIndex, notes[0]._lineLayer);
            // the other notes should be determined depending on one before note.
            for (var i = 1; i < n; i++)
            {
                SetCutDirection(notes[i - 1], notes[i]);
            }
        }

        void SetCutDirection(Note before, Note now)
        {
            var swingFac = Math.Pow(ConvertBeat(now._time - before._time) * 1.0 / EnoughIntervalMs, 1.0 / 2);
            if (swingFac > 1)
            {
                now._cutDirection = (int)PickBestDirectionSingle(now._lineIndex, now._lineLayer);
                now._cutDirection = (int)PickBestDirectionCont(now, before, swingFac);
            }
            else
            {
                now._cutDirection = (int)PickBestDirectionCont(now, before, swingFac);
            }
        }

        // the best cut direction for each section below
        //  8  9 10 11
        //  4  5  6  7
        //  0  1  2  3 
        int[] bestDir = new int[] { 6, 1, 1, 7, 2, 8, 8, 3, 4, 0, 0, 5 };
        CutDirection PickBestDirectionSingle(int line, int layer)
        {
            var idx = (int)Line.MaxNum * layer + line;
            var best = (CutDirection)bestDir[idx];
            if (best == CutDirection.Any) return PickRandomDirection();
            return CutDirection.Down;
        }

        CutDirection PickRandomDirection(DirectionRandomMode mode = DirectionRandomMode.Any)
        {
            int min = 0, max = (int)CutDirection.Any;
            switch (mode)
            {
                case DirectionRandomMode.OnlyNormal:
                    max = (int)CutDirection.Left + 1;
                    break;
                case DirectionRandomMode.OnlyDiagonal:
                    min = (int)CutDirection.DownRight;
                    break;
            }
            return (CutDirection)RandNumber(min, max);
        }

        // positon difference in each axis after the specified direction of cut
        static float Sqr2 = (float)Math.Sqrt(2) / 2;
        float[] lineDiff = new float[] { 0, 0, -1, 1, -Sqr2, Sqr2, -Sqr2, Sqr2 };
        float[] layerDiff = new float[] { 1, -1, 0, 0, Sqr2, Sqr2, -Sqr2, -Sqr2 };

        CutDirection PickBestDirectionCont(Note now, Note before, double swingAmount)
        {
            var lastcut = before._cutDirection;
            if (lastcut == (int)CutDirection.Any)
                lastcut = (int)PickRandomDirection();

            // limit factor
            swingAmount = Math.Max(swingAmount, 1.5);

            // this is where player's hand supposed to be
            var nowline = before._lineIndex + lineDiff[lastcut] * swingAmount;
            var nowlayer = before._lineLayer + layerDiff[lastcut] * swingAmount;

            var linegap = now._lineIndex - nowline;
            var layergap = now._lineLayer - nowlayer;
            var deg = Math.Atan2(layergap, linegap * 3.0 / 4) * 180 / Math.PI;

            CutDirection i;

            if (now._type == 0)
            {
                i = PickDirectionFromDegRed(deg);
            }
            else
            {
                i = PickDirectionFromDegBlue(deg);
            }

            //i = PickDirectionFromDeg(deg);

            if(before != null) //This will place notes in a way that we like, the converter will automatically give us the best next direction.
            {
                i = LogicChecker(now, before, i);
            }

            return i;
        }

        CutDirection PickDirectionFromDeg(double deg)
        {
            const double Div = 45;
            if (deg >= 180 - Div / 2) return CutDirection.Left;
            if (deg >= 180 - Div * 3 / 2) return CutDirection.UpLeft;
            if (deg >= 180 - Div * 5 / 2) return CutDirection.Up;
            if (deg >= 180 - Div * 7 / 2) return CutDirection.UpRight;
            if (deg >= 180 - Div * 9 / 2) return CutDirection.Right;
            if (deg >= 180 - Div * 11 / 2) return CutDirection.DownRight;
            if (deg >= 180 - Div * 13 / 2) return CutDirection.Down;
            if (deg >= 180 - Div * 15 / 2) return CutDirection.DownLeft;
            return CutDirection.Left;
        }

        CutDirection PickDirectionFromDegRed(double deg)
        {
            const double Div = 45;
            if (deg >= 180 - Div / 2) return CutDirection.UpLeft;
            if (deg >= 180 - Div * 3 / 2) return CutDirection.UpLeft;
            if (deg >= 180 - Div * 5 / 2) return CutDirection.Up;
            if (deg >= 180 - Div * 7 / 2) return CutDirection.Up;
            if (deg >= 180 - Div * 9 / 2) return CutDirection.DownRight;
            if (deg >= 180 - Div * 11 / 2) return CutDirection.DownRight;
            if (deg >= 180 - Div * 13 / 2) return CutDirection.Down;
            if (deg >= 180 - Div * 15 / 2) return CutDirection.Down;
            return CutDirection.UpLeft;
        }

        CutDirection PickDirectionFromDegBlue(double deg)
        {
            const double Div = 45;
            if (deg >= 180 - Div / 2) return CutDirection.DownLeft;
            if (deg >= 180 - Div * 3 / 2) return CutDirection.Up;
            if (deg >= 180 - Div * 5 / 2) return CutDirection.Up;
            if (deg >= 180 - Div * 7 / 2) return CutDirection.UpRight;
            if (deg >= 180 - Div * 9 / 2) return CutDirection.UpRight;
            if (deg >= 180 - Div * 11 / 2) return CutDirection.Down;
            if (deg >= 180 - Div * 13 / 2) return CutDirection.Down;
            if (deg >= 180 - Div * 15 / 2) return CutDirection.DownLeft;
            return CutDirection.DownLeft;
        }

        #endregion

        #region Process for adding symmetry notes
        void AddSymmetryNotes()
        {
            var n = Notes.Count;
            if (n < 2) return;

            var addingNotes = new List<Note>();
            SymmetryMode symmode = SymmetryMode.Line;

            AddSymmetryNote(null, Notes[0], Notes[1], addingNotes, symmode);
            for (var i = 1; i < Notes.Count - 1; i++)
            {
                var now = Notes[i];
                AddSymmetryNote(Notes[i - 1], now, Notes[i + 1], addingNotes, symmode);
            }
            AddSymmetryNote(Notes[n - 2], Notes[n - 1], null, addingNotes, symmode);

            foreach (var note in addingNotes)
            {
                notes.Add(note);
            }
            notes = Notes.OrderBy(note => note._time).ToList();
        }

        void AddSymmetryNote(Note before, Note now, Note after, List<Note> addition, SymmetryMode symmode)
        {
            double lastInterval = 0, nextInterval = 0;
            if (before == null)
                lastInterval = EnoughIntervalForSymMs * 2;
            else
                lastInterval = ConvertBeat(now._time - before._time);

            if (after == null)
                nextInterval = EnoughIntervalForSymMs * 2;
            else
                nextInterval = ConvertBeat(after._time - now._time);

            if (nextInterval > EnoughIntervalForSymMs && lastInterval > EnoughIntervalForSymMs)
            {
                var note = GetMirrorNote(now, symmode);
                if (before != null)
                {
                    note._cutDirection = (int)LogicChecker(note, before, (CutDirection)note._cutDirection);
                }
                addition.Add(note);
            }
        }

        Note GetMirrorNote(Note note, SymmetryMode mode)
        {
            int line = 0, layer = 0;
            switch (mode)
            {
                case SymmetryMode.Line:
                    line = (int)(-note._lineIndex + (int)Line.Right);
                    layer = note._lineLayer;
                    break;
                default:
                    line = (int)(-note._lineIndex + (int)Line.Right);
                    layer = (int)(-note._lineLayer + (int)Layer.Top);
                    break;
            }
            var dir = PickOppositeDirection(note._cutDirection, mode);
            var type = note._type == (int)NoteType.Blue ? NoteType.Red : NoteType.Blue;
            return new Note(note._time, line, layer, type, dir);
        }

        // the cut direction for symmetrically placed note
        int[] lineSym = new int[] { 0, 1, 3, 2, 5, 4, 7, 6 };
        int[] pointSym = new int[] { 1, 0, 3, 2, 7, 6, 5, 4 };
        CutDirection PickOppositeDirection(int dir, SymmetryMode mode)
        {
            if (dir < 0 || dir >= (int)CutDirection.Any)
                return CutDirection.Any;

            switch (mode)
            {
                case SymmetryMode.Line:
                    return (CutDirection)lineSym[dir];
                default:
                    return (CutDirection)pointSym[dir];
            }
        }
        #endregion

        #region Process for logic

        void LogicChecker()
        {
            notes = Notes.OrderBy(note => note._time).ToList();

            for (int i = 0; i < notes.Count() - 1; i++)
            {
                Note n = notes[i];

                if (notes[i + 1]._time - notes[i]._time <= 0.02 && notes[i + 1]._time - notes[i]._time >= -0.02)
                {
                    switch (n._cutDirection)
                    {
                        case 0: 
                            if(notes[i + 1]._lineIndex == n._lineIndex)
                            {
                                if (n._type == 0 && n._lineIndex != 0 && n._lineLayer != 1)
                                {
                                    notes[i]._lineIndex--;
                                }
                                else if (n._type == 1 && n._lineIndex != 3 && n._lineLayer != 1)
                                {
                                    notes[i]._lineIndex++;
                                }
                                else if(n._type == 0 && n._lineIndex == 0 && notes[i + 1]._lineLayer != 1)
                                {
                                    notes[i + 1]._lineIndex++;
                                }
                                else if(n._type == 1 && n._lineIndex == 3 && notes[i+1]._lineLayer != 1)
                                {
                                    notes[1 + 1]._lineIndex--;
                                }
                            }
                            break;
                        case 1:
                            if (notes[i + 1]._lineIndex == n._lineIndex)
                            {
                                if (n._type == 0 && n._lineIndex != 0 && n._lineLayer != 1)
                                {
                                    notes[i]._lineIndex--;
                                }
                                else if (n._type == 1 && n._lineIndex != 3 && n._lineLayer != 1)
                                {
                                    notes[i]._lineIndex++;
                                }
                                else if (n._type == 0 && n._lineIndex == 0 && notes[i + 1]._lineLayer != 1)
                                {
                                    notes[i + 1]._lineIndex++;
                                }
                                else if (n._type == 1 && n._lineIndex == 3 && notes[i + 1]._lineLayer != 1)
                                {
                                    notes[1 + 1]._lineIndex--;
                                }
                            }
                            break;
                        case 2:
                            if(notes[i + 1]._lineLayer == n._lineLayer)
                            {
                                if(n._lineLayer == 2 || n._lineLayer == 1)
                                {
                                    notes[i]._lineLayer--;
                                }
                                else
                                {
                                    notes[i]._lineLayer++;
                                }
                            }
                            break;
                        case 3:
                            if (notes[i + 1]._lineLayer == n._lineLayer)
                            {
                                if (n._lineLayer == 2 || n._lineLayer == 1)
                                {
                                    notes[i]._lineLayer--;
                                }
                                else
                                {
                                    notes[i]._lineLayer++;
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
                                }
                                else if ((notes[i + 1]._cutDirection == 4 || notes[i + 1]._cutDirection == 6) && notes[i + 1]._lineLayer != 2 && notes[i + 1]._lineIndex != 3)
                                {
                                    notes[i]._lineIndex = notes[i + 1]._lineIndex + 1;
                                    notes[i]._lineLayer = notes[i + 1]._lineLayer + 1;
                                }
                                else if(n._type == 1 && n._lineIndex != 3)
                                {
                                    notes[i]._lineIndex++;
                                }
                                else if (notes[i + 1]._type == 0 && notes[i + 1]._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex--;
                                }
                                else if(n._type == 0 && n._lineIndex != 0)
                                {
                                    notes[i]._lineIndex--;
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
                                }
                                else if ((notes[i + 1]._cutDirection == 5 || notes[i + 1]._cutDirection == 7) && notes[i + 1]._lineLayer != 2 && notes[i + 1]._lineIndex != 3)
                                {
                                    notes[i]._lineIndex = notes[i + 1]._lineIndex + 1;
                                    notes[i]._lineLayer = notes[i + 1]._lineLayer + 1;
                                }
                                else if (n._type == 1 && n._lineIndex != 3)
                                {
                                    notes[i]._lineIndex++;
                                }
                                else if (notes[i + 1]._type == 0 && notes[i + 1]._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex--;
                                }
                                else if (n._type == 0 && n._lineIndex != 0)
                                {
                                    notes[i]._lineIndex--;
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
                                }
                                else if ((notes[i + 1]._cutDirection == 4 || notes[i + 1]._cutDirection == 6) && notes[i + 1]._lineLayer != 2 && notes[i + 1]._lineIndex != 3)
                                {
                                    notes[i]._lineIndex = notes[i + 1]._lineIndex + 1;
                                    notes[i]._lineLayer = notes[i + 1]._lineLayer + 1;
                                }
                                else if (n._type == 1 && n._lineIndex != 3)
                                {
                                    notes[i]._lineIndex++;
                                }
                                else if (notes[i + 1]._type == 0 && notes[i + 1]._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex--;
                                }
                                else if (n._type == 0 && n._lineIndex != 0)
                                {
                                    notes[i]._lineIndex--;
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
                                }
                                else if ((notes[i + 1]._cutDirection == 5 || notes[i + 1]._cutDirection == 7) && notes[i + 1]._lineLayer != 2 && notes[i + 1]._lineIndex != 3)
                                {
                                    notes[i]._lineIndex = notes[i + 1]._lineIndex + 1;
                                    notes[i]._lineLayer = notes[i + 1]._lineLayer + 1;
                                }
                                else if (n._type == 1 && n._lineIndex != 3)
                                {
                                    notes[i]._lineIndex++;
                                }
                                else if (notes[i + 1]._type == 0 && notes[i + 1]._lineIndex != 0)
                                {
                                    notes[i + 1]._lineIndex--;
                                }
                                else if (n._type == 0 && n._lineIndex != 0)
                                {
                                    notes[i]._lineIndex--;
                                }
                            }
                            break;
                        case 8:
                            break;
                    }

                    if((n._lineIndex == 1 || n._lineIndex == 2) && n._lineLayer == 1)
                    {
                        if(n._cutDirection == 0 || n._cutDirection == 4 || n._cutDirection == 5)
                        {
                            notes[i]._lineLayer++;
                        }
                        else if(n._cutDirection == 1 || n._cutDirection == 6 || n._cutDirection == 7)
                        {
                            notes[i]._lineLayer--;
                        }
                        else
                        {
                            notes[i]._lineLayer--;
                        }
                    }
                }
            }

            for (int i = 0; i < notes.Count() - 1; i++)
            {
                if (notes[i + 1]._time - notes[i]._time <= 0.02 && notes[i + 1]._time - notes[i]._time >= -0.02)
                {
                    if ((notes[i]._lineIndex > notes[i + 1]._lineIndex + 1 && notes[i]._type == 0) || (notes[i]._lineIndex < notes[i + 1]._lineIndex - 1 && notes[i]._type == 1))
                    {
                        int tempo;
                        tempo = notes[i]._lineIndex;
                        notes[i]._lineIndex = notes[i + 1]._lineIndex;
                        notes[i + 1]._lineIndex = tempo;
                    }
                }
            }

                    //Fix hitbox issue
            for (int i = 0; i < notes.Count() - 1; i++)
            {
                Note n = notes[i];

                if (n._time == notes[i + 1]._time && (n._lineLayer == notes[i + 1]._lineLayer || n._lineLayer == notes[i + 1]._lineLayer - 1 || n._lineLayer == notes[i + 1]._lineLayer + 1) && (n._lineIndex == notes[i + 1]._lineIndex || n._lineIndex == notes[i + 1]._lineIndex + 1 || n._lineIndex == notes[i + 1]._lineIndex - 1))
                {
                    if(n._type == 0 && n._lineIndex <= 1)
                    {
                        notes[i + 1]._lineIndex = n._lineIndex + 2;
                    }
                    else if (n._type == 1 && n._lineIndex >= 2)
                    {
                        notes[i + 1]._lineIndex = n._lineIndex - 2;
                    }
                    else if (n._type == 1 && n._lineIndex <= 1)
                    {
                        notes[i]._lineIndex = notes[i + 1]._lineIndex + 2;
                    }
                    else if (n._type == 0 && n._lineIndex >= 2)
                    {
                        notes[i]._lineIndex = notes[i + 1]._lineIndex - 2;
                    }
                }
            }

            Note lastBlue = new Note(0, 0, 0, 0, 0);
            Note lastRed = new Note(0, 0, 0, 0, 0);

            //Fix shit-flow
            for (int i = 0; i < notes.Count() - 1; i++)
            {
                Note n = notes[i];

                if(lastBlue._cutDirection == 4 && n._cutDirection == 6 && n._type == 1)
                {
                    notes[i]._cutDirection = 1;
                }
                else if (lastBlue._cutDirection == 5 && n._cutDirection == 7 && n._type == 1)
                {
                    notes[i]._cutDirection = 1;
                }
                else if (lastBlue._cutDirection == 6 && n._cutDirection == 4 && n._type == 1)
                {
                    notes[i]._cutDirection = 0;
                }
                else if (lastBlue._cutDirection == 7 && n._cutDirection == 5 && n._type == 1)
                {
                    notes[i]._cutDirection = 0;
                }
                else if (lastRed._cutDirection == 5 && n._cutDirection == 7 && n._type == 0)
                {
                    notes[i]._cutDirection = 1;
                }
                else if (lastRed._cutDirection == 6 && n._cutDirection == 4 && n._type == 0)
                {
                    notes[i]._cutDirection = 0;
                }
                else if (lastRed._cutDirection == 7 && n._cutDirection == 5 && n._type == 0)
                {
                    notes[i]._cutDirection = 0;
                }
                else if (lastRed._cutDirection == 7 && n._cutDirection == 5 && n._type == 0)
                {
                    notes[i]._cutDirection = 0;
                }

                if (n._cutDirection == 5 && n._lineIndex == 0)
                {
                    notes[i]._cutDirection = 0;
                }
                else if (n._cutDirection == 4 && n._lineIndex == 3)
                {
                    notes[i]._cutDirection = 0;
                }
                else if (n._cutDirection == 6 && n._lineIndex == 3)
                {
                    notes[i]._cutDirection = 1;
                }
                else if (n._cutDirection == 7 && n._lineIndex == 0)
                {
                    notes[i]._cutDirection = 1;
                }

                if(n._type == 0)
                {
                    lastRed = notes[i];
                }
                else
                {
                    lastBlue = notes[i];
                }
            }
        }

        CutDirection LogicChecker(Note now, Note before, CutDirection i)
        {
            if(now._lineLayer == 2 && now._type == 0 && now._lineIndex == 2)
            {
                now._lineLayer = 0;
            }
            else if (now._lineLayer == 2 && now._type == 1 && now._lineIndex == 1)
            {
                now._lineLayer = 0;
            }
            //To make left/right notes bearable
            if (now._lineIndex == 0 && i == CutDirection.Right)
            {
                now._lineLayer = 0;
                now._lineIndex = 1;
                i = CutDirection.DownRight;
            }
            else if (now._lineIndex == 3 && i == CutDirection.Left)
            {
                now._lineLayer = 0;
                now._lineIndex = 2;
                i = CutDirection.DownLeft;
            }
            else if (now._lineIndex == 3 && i == CutDirection.Right && now._lineLayer != 1)
            {
                now._lineLayer = 1;
            }
            else if (now._lineIndex == 0 && i == CutDirection.Left && now._lineLayer != 1)
            {
                now._lineLayer = 1;
            }
            else if (now._lineIndex == 2 && i == CutDirection.Right && now._type == 1)
            {
                now._lineLayer = 1;
                now._lineIndex = 3;
            }
            else if (now._lineIndex == 1 && i == CutDirection.Left && now._type == 0)
            {
                now._lineLayer = 1;
                now._lineIndex = 0;
            }
            else if (now._lineIndex == 1 && i == CutDirection.Right && now._type == 0)
            {
                i = CutDirection.DownRight;
            }
            else if (now._lineIndex == 2 && i == CutDirection.Left && now._type == 1)
            {
                i = CutDirection.DownLeft;
            }

            //Rough Angle check
            if (before._cutDirection == (int)CutDirection.UpLeft && i == CutDirection.UpRight)
            {
                i = CutDirection.Down;
            }
            else if (before._cutDirection == (int)CutDirection.UpRight && i == CutDirection.UpLeft)
            {
                i = CutDirection.Down;
            }
            else if (before._cutDirection == (int)CutDirection.DownRight && i == CutDirection.DownLeft)
            {
                i = CutDirection.Up;
            }
            else if (before._cutDirection == (int)CutDirection.DownLeft && i == CutDirection.DownRight)
            {
                i = CutDirection.Up;
            }
            else if (before._cutDirection == (int)CutDirection.Up && i == CutDirection.UpLeft)
            {
                i = CutDirection.Down;
            }
            else if (before._cutDirection == (int)CutDirection.Up && i == CutDirection.UpRight)
            {
                i = CutDirection.Down;
            }
            else if (before._cutDirection == (int)CutDirection.Down && i == CutDirection.DownLeft)
            {
                i = CutDirection.Up;
            }
            else if (before._cutDirection == (int)CutDirection.Down && i == CutDirection.DownRight)
            {
                i = CutDirection.Up;
            }
            else if (before._cutDirection == (int)CutDirection.DownRight && i == CutDirection.Left)
            {
                i = CutDirection.Up;
            }
            else if (before._cutDirection == (int)CutDirection.DownLeft && i == CutDirection.Right)
            {
                i = CutDirection.Up;
            }
            else if (before._cutDirection == (int)CutDirection.Left && i == CutDirection.Up)
            {
                i = CutDirection.Down;
            }
            else if (before._cutDirection == (int)CutDirection.Right && i == CutDirection.Up)
            {
                i = CutDirection.Down;
            }
            else if(before._cutDirection == (int)CutDirection.UpRight && i == CutDirection.Up)
            {
                i = CutDirection.Down;
            }

            //Bad placement fixes
            if ((i == CutDirection.DownRight && now._lineLayer == 2) || (i == CutDirection.DownRight && now._lineLayer == 1))
            {
                now._lineLayer = 0;
            }
            else if ((i == CutDirection.DownLeft && now._lineLayer == 2) || (i == CutDirection.DownLeft && now._lineLayer == 1))
            {
                now._lineLayer = 0;
            }
            else if ((i == CutDirection.Down && now._lineLayer == 2) || (i == CutDirection.Down && now._lineLayer == 1))
            {
                now._lineLayer = 0;
            }
            else if (i == CutDirection.UpLeft && now._type == 1 && now._lineIndex == 3)
            {
                i = CutDirection.Up;
            }
            else if (i == CutDirection.UpRight && now._type == 0 && now._lineIndex == 0)
            {
                i = CutDirection.Up;
            }
            else if (i == CutDirection.UpRight && now._type == 1 && now._lineIndex == 3)
            {
                now._lineLayer = 1;
            }
            else if (i == CutDirection.UpRight && now._type == 1 && now._lineIndex == 2)
            {
                now._lineLayer = 2;
                now._lineIndex = 3;
            }
            else if (i == CutDirection.UpLeft && now._type == 0 && now._lineIndex == 0)
            {
                now._lineLayer = 1;
            }
            else if (i == CutDirection.UpLeft && now._type == 0 && now._lineIndex == 1)
            {
                now._lineLayer = 2;
                now._lineIndex = 0;
            }
            if (i == CutDirection.DownRight && now._type == 0 && now._lineIndex == 0 && before._lineIndex != 1)
            {
                now._lineIndex = 1;
            }
            else if (i == CutDirection.DownLeft && now._type == 1 && now._lineIndex == 3 && before._lineIndex != 2)
            {
                now._lineIndex = 2;
            }

            //Double directional fixes
            if (before._cutDirection == (int)i)
            {
                if (i == CutDirection.UpLeft)
                {
                    i = CutDirection.Down;
                }
                else if (i == CutDirection.DownRight)
                {
                    i = CutDirection.Up;
                }
                else if (i == CutDirection.DownLeft)
                {
                    i = CutDirection.Up;
                }
                else if (i == CutDirection.UpRight)
                {
                    i = CutDirection.Down;
                }
                else if (i == CutDirection.Down)
                {
                    i = CutDirection.Up;
                }
                else if (i == CutDirection.Up)
                {
                    i = CutDirection.Down;
                }
                else if (i == CutDirection.Left)
                {
                   i = CutDirection.Right;
                }
                else if (i == CutDirection.Right)
                {
                    i = CutDirection.Left;
                }
            }
            
            switch (now._type)
            {
                case 0:
                    if (before._cutDirection == (int)CutDirection.UpLeft && i == CutDirection.Up)
                    {
                        i = CutDirection.Down;
                    }
                    else if (before._cutDirection == (int)CutDirection.Up && i == CutDirection.UpLeft)
                    {
                        i = CutDirection.Down;
                    }
                    else if (before._cutDirection == (int)CutDirection.DownLeft && i == CutDirection.Down)
                    {
                        i = CutDirection.Up;
                    }
                    else if (before._cutDirection == (int)CutDirection.Down && i == CutDirection.Down)
                    {
                        i = CutDirection.Up;
                    }
                    else if(before._cutDirection == (int)CutDirection.Left && i == CutDirection.DownLeft)
                    {
                        i = CutDirection.Right;
                    }
                    break;
                case 1:
                    if (before._cutDirection == (int)CutDirection.DownRight && i == CutDirection.Down)
                    {
                        i = CutDirection.Up;
                    }
                    else if (before._cutDirection == (int)CutDirection.UpRight && i == CutDirection.Up)
                    {
                        i = CutDirection.Down;
                    }
                    else if (before._cutDirection == (int)CutDirection.Down && i == CutDirection.DownRight)
                    {
                        i = CutDirection.Up;
                    }
                    else if (before._cutDirection == (int)CutDirection.Down && i == CutDirection.Down)
                    {
                        i = CutDirection.Up;
                    }
                    else if (before._cutDirection == (int)CutDirection.Right && i == CutDirection.DownRight)
                    {
                        i = CutDirection.Left;
                    }
                    break;
            }

            return i;
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
            double tempTime;
            bool jumpTime = false;
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
                    jumpTime = false;

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
                        jumpTime = true;
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
                                    jumpTime = true;
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

                        /*if (j > 1 && RandNumber(0, 2) == 0 && patternStart != 0 && jumpTime == true && notes[patternStart + j - 1] != lastAddedNote && note._type != notes[patternStart + j - 1]._type && note._time - notes[patternStart + j - 1]._time > 0.50 && j != available - 1 && notes[patternStart + j + 1]._time - note._time > 0.25)
                        {
                            swapTime.Add(notes[patternStart + j - 1]);
                            swapTime.Add(note);
                            lastAddedNote = note;
                        }*/

                        notes[patternStart + j] = note;
                        
                        /*if (note._time - before._time <= 0.01)
                        {
                            note._lineLayer = 0;
                            before._lineLayer = 0;
                            if (before._cutDirection == 6 || before._cutDirection == 7)
                            {
                                before._cutDirection = 1;
                            }
                            else if (before._cutDirection == 4 || before._cutDirection == 5)
                            {
                                before._cutDirection = 0;
                            }
                            if (note._cutDirection == 6 || note._cutDirection == 7)
                            {
                                note._cutDirection = 1;
                            }
                            else if (note._cutDirection == 4 || note._cutDirection == 5)
                            {
                                note._cutDirection = 0;
                            }
                            if (note._type == 1)
                            {
                                note._lineIndex = 2;
                                before._lineIndex = 1;
                            }
                            else
                            {
                                note._lineIndex = 1;
                                before._lineIndex = 2;
                            }
                        }*/

                        countFix++;
                        looper++;
                        if (countFix == 4)
                        {
                            countFix = 0;
                        }

                        before = note;
                    }
                    jumpTime = false;
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

            /*Note temp;
            List<Note> tempo = new List<Note>();

            if (swapTime.Any())
            {
                for (int j = 0; j < swapTime.Count; j = j + 2)
                {
                    tempTime = swapTime[j]._time;
                    swapTime[j]._time = swapTime[j + 1]._time;
                    swapTime[j + 1]._time = tempTime;
                }
            }

            notes = Notes.OrderBy(note => note._time).ToList();
            bool find = false;

            for (int i = 2; i < notes.Count; i++)
            {
                if(notes[i]._time - notes[i - 1]._time > LimitStacked)
                {
                    temp = new Note(notes[i - 1]);
                    for(int j = 0; j < swapTime.Count; j++)
                    {
                        if(temp == swapTime[j])
                        {
                            find = true;
                        }
                    }

                    if(find == false)
                    {
                        tempo.Add(AddStackedNote(notes[i - 1], temp));
                    }
                    find = false;
                }
            }

            for(int i = 0; i < tempo.Count; i++)
            {
                notes.Add(tempo[i]);
            }

            notes = Notes.OrderBy(note => note._time).ToList();*/

            if(notes.Any())
            {
                notes.RemoveAt(notes.Count() - 1);
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

        Note AddStackedNote(Note before, Note temp)
        {
            if (temp._cutDirection == 0 || temp._cutDirection == 1)
            {
                if (temp._lineLayer == 0 || temp._lineLayer == 1)
                {
                    temp._lineLayer++;
                }
                else
                {
                    temp._lineLayer--;
                }
            }
            else if (temp._cutDirection == 6)
            {
                if (temp._lineIndex == 0)
                {
                    if (temp._lineLayer == 1 || temp._lineLayer == 0)
                    {
                        temp._lineLayer++;
                        temp._lineIndex++;
                    }
                }
                else
                {
                    if (temp._lineLayer == 0)
                    {
                        if (temp._lineIndex != 3)
                        {
                            temp._lineIndex++;
                            temp._lineLayer++;
                        }
                    }
                    else
                    {
                        temp._lineLayer--;
                        temp._lineIndex--;
                    }
                }
            }
            else if (temp._cutDirection == 7)
            {
                if (temp._lineIndex == 3)
                {
                    if (temp._lineLayer == 1 || temp._lineLayer == 0)
                    {
                        temp._lineLayer++;
                        temp._lineIndex--;
                    }                
                }
                else
                {
                    if (temp._lineLayer == 0)
                    {
                        if(temp._lineIndex != 0)
                        {
                            temp._lineIndex--;
                            temp._lineLayer++;
                        }
                    }
                    else
                    {
                        temp._lineLayer--;
                        temp._lineIndex++;
                    }
                }
            }
            else if (temp._cutDirection == 2)
            {
                if(temp._lineIndex != 0)
                {
                    temp._lineIndex--;
                }
                else
                {
                    temp._lineIndex++;
                }
            }
            else if (temp._cutDirection == 3)
            {
                if (temp._lineIndex != 3)
                {
                    temp._lineIndex++;
                }
                else
                {
                    temp._lineIndex--;
                }
            }
            else if (temp._cutDirection == 4)
            {
                if (temp._lineIndex == 0)
                {
                    if (temp._lineLayer != 0)
                    {
                        temp._lineLayer--;
                        temp._lineIndex++;
                    }
                }
                else
                {
                    if (temp._lineLayer == 0)
                    {
                        temp._lineLayer++;
                        temp._lineIndex--;
                    }
                    else
                    {
                        temp._lineLayer--;
                        temp._lineIndex++;
                    }
                }
            }
            else if (temp._cutDirection == 5)
            {
                if (temp._lineIndex == 3)
                {
                    if (temp._lineLayer != 0)
                    {
                        temp._lineLayer--;
                        temp._lineIndex--;
                    }
                }
                else
                {
                    if (temp._lineLayer == 0)
                    {
                        temp._lineLayer++;
                        temp._lineIndex++;
                    }
                    else
                    {
                        temp._lineLayer--;
                        temp._lineIndex--;
                    }
                }
            }

            if((temp._lineIndex == 1 || temp._lineIndex == 2) && temp._lineLayer == 1)
            {
                if(before._lineLayer == 0)
                {
                    temp._lineLayer = 2;
                }
                else
                {
                    temp._lineLayer = 0;
                }
                if (temp._cutDirection == 6 || temp._cutDirection == 7)
                {
                    before._cutDirection = 1;
                }
                else if (temp._cutDirection == 4 || temp._cutDirection == 5)
                {
                    before._cutDirection = 0;
                }
            }

            /*if((temp._cutDirection == 1 || temp._cutDirection == 6 || temp._cutDirection == 7) && temp._lineLayer == 2)
            {
                temp._lineLayer = 1;
            }
            if((before._cutDirection == 1 || before._cutDirection == 6 || before._cutDirection == 7) && before._lineLayer == 2)
            {
                before._lineLayer = 1;
            }*/

            /*if(temp._lineLayer == 0 && (temp._cutDirection == 1 || temp._cutDirection == 6 || temp._cutDirection == 7))
            {
                temp._time += 0.03125;
            }
            else if (before._lineLayer == 0 && (before._cutDirection == 1 || before._cutDirection == 6 || before._cutDirection == 7))
            {
                before._time += 0.03125;
            }
            else if (temp._lineLayer == 2 && (temp._cutDirection == 0 || temp._cutDirection == 4 || temp._cutDirection == 5))
            {
                temp._time += 0.03125;
            }
            else if (before._lineLayer == 2 && (before._cutDirection == 0 || before._cutDirection == 4 || before._cutDirection == 5))
            {
                before._time += 0.03125;
            }
            else if (temp._lineIndex == 0 && (temp._cutDirection == 2 || temp._cutDirection == 4 || temp._cutDirection == 6))
            {
                temp._time += 0.03125;
            }
            else if (before._lineIndex == 0 && (before._cutDirection == 2 || before._cutDirection == 4 || before._cutDirection == 6))
            {
                before._time += 0.03125;
            }
            else if (temp._lineIndex == 3 && (temp._cutDirection == 3 || temp._cutDirection == 5 || temp._cutDirection == 7))
            {
                temp._time += 0.03125;
            }
            else if (before._lineIndex == 3 && (before._cutDirection == 3 || before._cutDirection == 5 || before._cutDirection == 7))
            {
                before._time += 0.03125;
            }*/

            return temp;
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

    enum DirectionRandomMode
    {
        Any,
        OnlyNormal,
        OnlyDiagonal
    }

    enum SymmetryMode
    {
        Line,
        Point
    }
}
