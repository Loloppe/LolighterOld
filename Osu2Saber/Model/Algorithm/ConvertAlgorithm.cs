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
        public static bool AllowOneHanded = false;
        public static bool DoubleHitboxFix = true;

        protected const float OsuScreenXMax = 512, OsuScreenYMax = 384;

        public static double SlowSpeed = 0.4;
        public static double ParitySpeed = 10;
        public static double EnoughIntervalBetweenNotes = 0.2;
        public static double GallopSpeed = 0.3;
        public static string PatternToUse = "Pack";

        protected Beatmap org;
        protected SaberBeatmap dst;

        protected List<Event> events = new List<Event>();
        protected List<Obstacle> obstacles = new List<Obstacle>();
        protected List<Note> notes = new List<Note>();
        protected List<Note> mines = new List<Note>();
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
        public List<Note> Mines => mines;

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
            foreach (var n in note)
            {
                if(n._type != 3)
                {
                    Notes.Add(n);
                }
                else if(n._type == 3)
                {
                    Mines.Add(n);
                }
            }
            
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

        #region Process for patterns
        private void MapReader()
        {
            if(PatternToUse != "Pack" && PatternToUse != "RandomStream" && PatternToUse != "Complex")
            {
                PatternToUse = "Pack";
            }
            if(PatternToUse == "Pack")
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Pack");
                MessageBox.Show("Select the pack to use for faster part.");
                OpenFileDialog open = new OpenFileDialog
                {
                    Filter = "pak|*.pak",
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Pack",
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
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Pack",
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

        // Last direction
        private int leftHand = 0;
        private int rightHand = 0;
        private bool foundBlue = false;
        private bool foundRed = false;

        void PatternCreator(string pattern)
        {
            // Nb of attempt to get a pattern that fit, if > 100000, forcefully stop the process.
            int attempt = 0;
            int available = notes.Count;
            double first = notes[0]._time;
            // Current slow section
            bool slow = false;
            // To know the placement order of notes.
            List<int> noteOrder = new List<int>();
            // Note to be used from pattern
            Queue<Note> blueNote = new Queue<Note>();
            Queue<Note> redNote = new Queue<Note>();
            // To convert to queue
            List<Note> patternList = new List<Note>();
            // Last-Last direction
            int leftHand2 = 1;
            int rightHand2 = 1;
            // Last note used
            Note preceding = new Note(-1, -1, -1, -1, -1);

            Note n = new Note(-1, -1, -1, NoteType.Mine, (CutDirection)8);

            //For each notes
            for (int i = 0; i < available - 2; i++)
            {
                n = notes[i];

                // If the pattern is done or the speed got faster than slow (ignore double)
                if (noteOrder.Count() == 0 || (noteOrder.Count % 2 == 0 && slow && notes[i + 1]._time - notes[i]._time < SlowSpeed * (bpm / bpmPerNote[i]) && notes[i + 1]._time - notes[i]._time >= 0.01))
                {
                    // Infinite loop until a pattern that fit the condition is met.
                    do
                    {
                        // Need to find a new pattern.
                        foundBlue = false;
                        foundRed = false;
                        slow = false;
                        attempt++;
                        // Clear/create a new loop.
                        blueNote.Clear();
                        redNote.Clear();
                        noteOrder.Clear();

                        if(attempt >= 100000)
                        {
                            MessageBox.Show("Can't find a proper pattern, closing.");
                            Process.GetCurrentProcess().Kill();
                        }

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

                        // Here we check the flow before parity
                        if (!(notes[i + 1]._time - notes[i]._time < 0.02 && notes[i + 1]._time - notes[i]._time > -0.02))
                        {
                            // We ignore double for this
                            if (ParitySpeed * (bpm / bpmPerNote[i]) > notes[i + 1]._time - notes[i]._time)
                            {
                                FlowCheck(redNote, 0);
                                FlowCheck(blueNote, 1);
                            }
                            else // Skip flow check
                            {
                                if(blueNote.Any())
                                {
                                    foundBlue = true;
                                }
                                if(redNote.Any())
                                {
                                    foundRed = true;
                                }
                            }
                        }
                        else // It's a double
                        {
                            if (ParitySpeed * (bpm / bpmPerNote[i]) > notes[i + 2]._time - notes[i + 1]._time)
                            {
                                FlowCheck(redNote, 0);
                                FlowCheck(blueNote, 1);
                            }
                            else // Skip flow check
                            {
                                if (blueNote.Any())
                                {
                                    foundBlue = true;
                                }
                                if (redNote.Any())
                                {
                                    foundRed = true;
                                }
                            }
                        }
                            

                        // We only check parity if it's faster than X beat (for lower diff)
                        if(foundRed && foundBlue)
                        {
                            // We ignore double for this
                            if (!(notes[i + 1]._time - notes[i]._time < 0.02 && notes[i + 1]._time - notes[i]._time > -0.02))
                            {
                                if (ParitySpeed * (bpm / bpmPerNote[i]) > notes[i + 1]._time - notes[i]._time)
                                {
                                    if (!ParityCheck(redNote.Peek()._type, redNote.Peek()._cutDirection, leftHand, leftHand2))
                                    {
                                        foundRed = false;
                                    }
                                    if (!ParityCheck(blueNote.Peek()._type, blueNote.Peek()._cutDirection, rightHand, rightHand2))
                                    {
                                        foundBlue = false;
                                    }
                                }
                            }
                            else // It's a double
                            {
                                if (ParitySpeed * (bpm / bpmPerNote[i]) > notes[i + 2]._time - notes[i + 1]._time)
                                {
                                    if (!ParityCheck(redNote.Peek()._type, redNote.Peek()._cutDirection, leftHand, leftHand2))
                                    {
                                        foundRed = false;
                                    }
                                    if (!ParityCheck(blueNote.Peek()._type, blueNote.Peek()._cutDirection, rightHand, rightHand2))
                                    {
                                        foundBlue = false;
                                    }
                                }
                            }
                        }


                        // For patterns with only one color
                        if (((foundRed) || (foundBlue)) && AllowOneHanded)
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

                // Check for fused notes issue and fix hitbox issue
                if(n._time - preceding._time >= -0.02 && n._time - preceding._time <= 0.02)
                {
                    // Attempt to fix fused notes
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

                    // We only try to fix if the user decide to.
                    if(DoubleHitboxFix && i > 1)
                    {

                        // Attempt to fix vision issue
                        if ((n._lineIndex == 1 || n._lineIndex == 2) && n._lineLayer == 1)
                        {
                            n._lineLayer++;
                        }
                        else if ((preceding._lineIndex == 1 || preceding._lineIndex == 2) && preceding._lineLayer == 1)
                        {
                            preceding._lineLayer++;
                        }

                        // No pickles top row
                        if (((n._type == 0 && n._lineIndex > preceding._lineIndex) || (n._type == 1 && n._lineIndex < preceding._lineIndex)) && n._lineLayer > 0)
                        {
                            int temp = n._lineIndex;
                            n._lineIndex = preceding._lineIndex;
                            preceding._lineIndex = temp;
                        }

                        // We lower a note if it fit
                        if(n._lineLayer == 2 && preceding._lineLayer == 2)
                        {
                            if(n._cutDirection == 1 && preceding._cutDirection == 1)
                            {
                                n._lineLayer = 0;
                                preceding._lineLayer = 0;
                            }
                            else if(n._cutDirection == 6 || n._cutDirection == 7)
                            {
                                n._lineLayer = 0;
                            }
                            else if(preceding._cutDirection == 6 || preceding._cutDirection == 7)
                            {
                                preceding._lineLayer = 0;
                            }
                        }
                        // We bring up a note if it fit
                        else if (n._lineLayer == 0 && preceding._lineLayer == 0)
                        {
                            if (n._cutDirection == 0 && preceding._cutDirection == 0)
                            {
                                n._lineLayer = 2;
                                preceding._lineLayer = 2;
                            }
                            if (n._cutDirection == 4 || n._cutDirection == 5)
                            {
                                n._lineLayer = 2;
                            }
                            else if (preceding._cutDirection == 4 || preceding._cutDirection == 5)
                            {
                                preceding._lineLayer = 2;
                            }
                        }
                        // Note side by side
                        if ((n._lineIndex == preceding._lineIndex - 1 || n._lineIndex == preceding._lineIndex + 1) && n._lineLayer == preceding._lineLayer && (n._cutDirection > 1 || preceding._cutDirection > 1))
                        {
                            // If the one before wasn't left or right
                            if(notes[i - 2]._cutDirection != 2 && notes[i - 2]._cutDirection != 3 && notes[i - 3]._cutDirection != 2 && notes[i - 3]._cutDirection != 3)
                            {
                                if (n._cutDirection == 6 || n._cutDirection == 7) // Modify to down
                                {
                                    n._cutDirection = 1;
                                }
                                else if (n._cutDirection == 4 || n._cutDirection == 5) // Modify to up
                                {
                                    n._cutDirection = 0;
                                }
                                if (preceding._cutDirection == 6 || preceding._cutDirection == 7)
                                {
                                    preceding._cutDirection = 1; // Modify to down
                                    if (preceding._type == 0) // Need to update for parity
                                    {
                                        leftHand = 1;
                                    }
                                    else if (preceding._type == 1)
                                    {
                                        rightHand = 1;
                                    }
                                }
                                else if (preceding._cutDirection == 4 || preceding._cutDirection == 5)
                                {
                                    preceding._cutDirection = 0; // Modify to up
                                    if (preceding._type == 0) // Need to update for parity
                                    {
                                        leftHand = 0;
                                    }
                                    else if (preceding._type == 1)
                                    {
                                        rightHand = 0;
                                    }
                                }
                            }
                            else // We separate them
                            {
                                if(n._lineIndex < preceding._lineIndex && n._lineIndex != 0)
                                {
                                    n._lineIndex--;
                                }
                                else if(n._lineIndex > preceding._lineIndex && n._lineIndex != 3)
                                {
                                    n._lineIndex++;
                                }
                                else if (preceding._lineIndex < n._lineIndex && preceding._lineIndex != 0)
                                {
                                    preceding._lineIndex--;
                                }
                                else if (preceding._lineIndex > n._lineIndex && preceding._lineIndex != 3)
                                {
                                    preceding._lineIndex++;
                                }
                            }
                        }
                        // Close together diagonally
                        if (((n._lineLayer == preceding._lineLayer - 1) || (n._lineLayer == preceding._lineLayer + 1)) && ((n._lineIndex == preceding._lineIndex - 1) || (n._lineIndex == preceding._lineIndex + 1)))
                        {
                            // Some can be skipped
                            if (n._cutDirection == preceding._cutDirection && ((n._cutDirection == 4 && n._lineIndex < 2 && n._lineLayer > 0 && preceding._lineLayer > 0) || (n._cutDirection == 5 && n._lineIndex > 1 && n._lineLayer > 0 && preceding._lineLayer > 0) || (n._cutDirection == 6 && n._lineIndex < 2 && n._lineLayer < 2 && preceding._lineLayer < 2) || (n._cutDirection == 7 && n._lineIndex > 1 && n._lineLayer < 2 && preceding._lineLayer < 2) || n._cutDirection < 4))
                            {
                                // Skip
                            }
                            else 
                            {
                                if (n._lineIndex == 0 || n._lineIndex == 3)
                                {
                                    if (n._cutDirection == 0 || n._cutDirection == 4 || n._cutDirection == 5)
                                    {
                                        n._lineLayer = 2;
                                        preceding._lineLayer = 0;
                                    }
                                    else
                                    {
                                        if(n._cutDirection != 2 && n._cutDirection != 3)
                                        {
                                            n._lineLayer = 0;
                                        }
                                        if(preceding._cutDirection == 0 || preceding._cutDirection == 4 || preceding._cutDirection == 5)
                                        {
                                            preceding._lineLayer = 2;
                                        }
                                        else
                                        {
                                            preceding._lineLayer = 0;
                                        }
                                    }
                                }
                                else if (preceding._lineIndex == 0 || preceding._lineIndex == 3)
                                {
                                    if (preceding._cutDirection == 0 || preceding._cutDirection == 4 || preceding._cutDirection == 5)
                                    {
                                        preceding._lineLayer = 2;
                                        n._lineLayer = 0;
                                    }
                                    else
                                    {
                                        if (preceding._cutDirection != 2 && preceding._cutDirection != 3)
                                        {
                                            preceding._lineLayer = 0;
                                        }
                                        if (n._cutDirection == 0 || n._cutDirection == 4 || n._cutDirection == 5)
                                        {
                                            n._lineLayer = 2;
                                        }
                                        else
                                        {
                                            n._lineLayer = 0;
                                        }
                                    }
                                }
                            }
                        }
                        // On top of eachother with a down or up.
                        if ((n._cutDirection < 2 || preceding._cutDirection < 2) && n._lineIndex == preceding._lineIndex)
                        {
                            if (n._type == 0 && n._lineIndex != 0)
                            {
                                n._lineIndex--;
                            }
                            else if (n._type == 1 && n._lineIndex != 3)
                            {
                                n._lineIndex++;
                            }
                        }
                        // Better flow with diagonal cross
                        if(n._type == 0 && n._lineLayer == 0 && n._lineIndex < 2 && preceding._lineLayer == 2 && preceding._lineIndex > 1)
                        {
                            if(n._cutDirection == 7 && preceding._cutDirection == 6)
                            {
                                n._lineIndex = 2;
                                preceding._lineIndex = 1;
                            }
                        }
                        else if (n._type == 1 && n._lineLayer == 2 && n._lineIndex > 1 && preceding._lineLayer == 0 && preceding._lineIndex < 2)
                        {
                            if (n._cutDirection == 6 && preceding._cutDirection == 7)
                            {
                                n._lineIndex = 1;
                                preceding._lineIndex = 2;
                            }
                        }
                    }
                }

                // Down into side doesn't flow, not sure why this is happening, so we fix here.
                if(i > 1)
                {
                    if ((n._cutDirection == 2 || n._cutDirection == 3) && notes[i - 2]._cutDirection == 1 && ParitySpeed * (bpm / bpmPerNote[i]) > notes[i + 1]._time - notes[i]._time)
                    {
                        if (n._cutDirection == 2)
                        {
                            n._cutDirection = 4;
                        }
                        if (n._cutDirection == 3)
                        {
                            n._cutDirection = 5;
                        }
                    }

                    // Make the flow a little smoother.
                    if (n._type == 0 && notes[i - 2]._lineIndex > n._lineIndex && notes[i - 2]._lineIndex > 1)
                    {
                        if (n._cutDirection == 7)
                        {
                            n._cutDirection = 1;
                        }
                        else if (n._cutDirection == 5)
                        {
                            n._cutDirection = 0;
                        }
                    }
                    else if (n._type == 1 && notes[i - 2]._lineIndex < n._lineIndex && notes[i - 2]._lineIndex < 2)
                    {
                        if (n._cutDirection == 6)
                        {
                            n._cutDirection = 1;
                        }
                        else if (n._cutDirection == 4)
                        {
                            n._cutDirection = 0;
                        }
                    }
                }
                

                // Always start the map on a bottom row down.
                if (n._time == first)
                {
                    n._cutDirection = 1;
                    if(n._type == 0)
                    {
                        n._lineIndex = 1;
                    }
                    else
                    {
                        n._lineIndex = 2;
                    }
                    n._lineLayer = 0;
                }

                // We add the note here
                notes[i] = new Note(n);
                if (i > 0)
                {
                    notes[i - 1] = new Note(preceding);
                }

                // Follow up the flow per hand
                if (n._type == 0)
                {
                    leftHand2 = leftHand;
                    leftHand = n._cutDirection;
                }
                else if(n._type == 1)
                {
                    rightHand2 = rightHand;
                    rightHand = n._cutDirection;
                }

                attempt = 0;
                preceding = new Note(notes[i]);
            }

            // Set notes by time order.
            notes = Notes.OrderBy(note => note._time).ToList();

            if (notes.Any() && notes.Count() > 3) // Fix the last 2 notes
            {
                notes[notes.Count() - 1]._type = notes[notes.Count() - 3]._type;
                notes[notes.Count() - 2]._type = notes[notes.Count() - 4]._type;
                notes[notes.Count() - 1]._cutDirection = 8;
                notes[notes.Count() - 2]._cutDirection = 8;
                notes[notes.Count() - 1]._lineLayer = 0;
                notes[notes.Count() - 2]._lineLayer = 0;
                if (notes[notes.Count() - 1]._type == 0)
                {
                    notes[notes.Count() - 1]._lineIndex = 1;
                    notes[notes.Count() - 2]._lineIndex = 2;
                }
                else
                {
                    notes[notes.Count() - 1]._lineIndex = 2;
                    notes[notes.Count() - 2]._lineIndex = 1;
                }
            }

            // Re-add the mines.
            notes.AddRange(mines);

            // Set notes by time order.
            notes = Notes.OrderBy(note => note._time).ToList();
        }

        Queue<Note> FlowCheck(Queue<Note> note, int type)
        {
            var hand = 0;

            if(type == 0)
            {
                hand = leftHand;
            }
            else if (type == 1)
            {
                hand = rightHand;
            }

            for (var i = 0; i < note.Count(); i++)
            {
                if (DownCut.Contains(hand) && UpCut.Contains(note.Peek()._cutDirection))
                {
                    if(type == 0)
                    {
                        foundRed = true;
                    }
                    else if(type == 1)
                    {
                        foundBlue = true;
                    }
                    
                    break;
                }
                else if (UpCut.Contains(hand) && DownCut.Contains(note.Peek()._cutDirection))
                {
                    if (type == 0)
                    {
                        foundRed = true;
                    }
                    else if (type == 1)
                    {
                        foundBlue = true;
                    }

                    break;
                }
                else if (hand == 2 && IntoLeft.Contains(note.Peek()._cutDirection))
                {
                    if (type == 0)
                    {
                        foundRed = true;
                    }
                    else if (type == 1)
                    {
                        foundBlue = true;
                    }

                    break;
                }
                else if (hand == 3 && IntoRight.Contains(note.Peek()._cutDirection))
                {
                    if (type == 0)
                    {
                        foundRed = true;
                    }
                    else if (type == 1)
                    {
                        foundBlue = true;
                    }

                    break;
                }
                else if (IntoLeft.Contains(hand) && note.Peek()._cutDirection == 2)
                {
                    if (type == 0)
                    {
                        foundRed = true;
                    }
                    else if (type == 1)
                    {
                        foundBlue = true;
                    }

                    break;
                }
                else if (IntoRight.Contains(hand) && note.Peek()._cutDirection == 3)
                {
                    if (type == 0)
                    {
                        foundRed = true;
                    }
                    else if (type == 1)
                    {
                        foundBlue = true;
                    }

                    break;
                }
                note.Enqueue(note.Dequeue());
            }

            return note;
        }

        bool ParityCheck(int type, int now, int before, int beforeBefore)
        {
            if(now == 8) // Any
            {
                return true;
            }

            // Also have to handle break in parity, hence the non-flow part.

            switch (beforeBefore)
            {
                case 0: // Up
                    switch (before)
                    {
                        case 0: // Up
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 1: // Down
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    if(type == 0) // Red
                                    {
                                        return true;
                                    }
                                    break;
                                case 5: // Up-Right
                                    if(type == 1) // Blue
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 2: // Left
                            switch (now)
                            {
                                case 3: // Right
                                    return true;
                                case 7: // Down-Right
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 3: // Right
                            switch (now)
                            {
                                case 2: // Left
                                    return true;
                                case 6: // Down-Left
                                    if (type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 4: // Up-Left
                            switch (now)
                            {
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 5: // Up-Right
                            switch (now)
                            {
                                case 6: // Down-Left
                                    return true;
                            }
                            break;
                        case 6: // Down-Left
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 7: // Down-Right
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                            }
                            break;
                        case 8: // Any
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                    }
                    break;
                case 1: // Down
                    switch (before)
                    {
                        case 0: // Up
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 1: // Down
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 2: // Left
                            switch (now)
                            {
                                case 3: // Right
                                    return true;
                                case 7: // Down-Right
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 3: // Right
                            switch (now)
                            {
                                case 2: // Left
                                    return true;
                                case 6: // Down-Left
                                    if (type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 4: // Up-Left
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 5: // Up-Right
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                            }
                            break;
                        case 6: // Down-Left
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 7: // Down-Right
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                            }
                            break;
                        case 8: // Any
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                    }
                    break;
                case 2: // Left
                    switch (before)
                    {
                        case 0: // Up
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 1: // Down
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 2: // Left
                            switch (now)
                            {
                                case 3: // Right
                                    return true;
                                case 7: // Down-Right
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 3: // Right
                            switch (now)
                            {
                                case 2: // Left
                                    return true;
                                case 4: // Up-Left
                                    if(type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                                case 6: // Down-Left
                                    if(type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 4: // Up-Left
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 5: // Up-Right
                            switch (now)
                            {
                                case 1: // Down
                                    if(type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                                case 6: // Down-Left
                                    if(type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 6: // Down-Left
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 7: // Down-Right
                            switch (now)
                            {
                                case 0: // Up
                                    if(type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                                case 2: // Left
                                    if(type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                                case 4: // Up-Left
                                    if(type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 8: // Any
                            switch (now)
                            {
                                case 2: // Left
                                    return true;
                            }
                            break;
                    }
                    break;
                case 3: // Right
                    switch (before)
                    {
                        case 0: // Up
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 1: // Down
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 2: // Left
                            switch (now)
                            {
                                case 3: // Right
                                    return true;
                                case 5: // Up-Right
                                    if (type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                                case 7: // Down-Right
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 3: // Right
                            switch (now)
                            {
                                case 2: // Left
                                    return true;
                                case 6: // Down-Left
                                    if (type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 4: // Up-Left
                            switch (now)
                            {
                                case 1: // Down
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                                case 7: // Down-Right
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 5: // Up-Right
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                            }
                            break;
                        case 6: // Down-Left
                            switch (now)
                            {
                                case 0: // Up
                                    if (type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                                case 3: // Right
                                    if (type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                                case 5: // Up-Right
                                    if (type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 7: // Down-Right
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                            }
                            break;
                        case 8: // Any
                            switch (now)
                            {
                                case 3: // Right
                                    return true;
                            }
                            break;
                    }
                    break;
                case 4: // Up-Left
                    switch (before)
                    {
                        case 0: // Up
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 1: // Down
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 2: // Left
                            switch (now)
                            {
                                case 3: // Right
                                    return true;
                                case 7: // Down-Right
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 3: // Right
                            switch (now)
                            {
                                case 2: // Left
                                    if(type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                                case 4: // Up-Left
                                    if(type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 4: // Up-Left
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 5: // Up-Right
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                            }
                            break;
                        case 6: // Down-Left
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 7: // Down-Right
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 2: // Left
                                    if(type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                                case 4: // Up-Left
                                    return true;
                            }
                            break;
                        case 8: // Any
                            switch (now)
                            {
                                case 4: // Up-Left
                                    return true;
                            }
                            break;
                    }
                    break;
                case 5: // Up-Right
                    switch(before)
                    {
                        case 0: // Up
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 1: // Down
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 2: // Left
                            switch (now)
                            {
                                case 3: // Right
                                    if (type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                                case 4: // Up-Left
                                    if (type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 3: // Right
                            switch (now)
                            {
                                case 2: // Left
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                                case 4: // Up-Left
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 4: // Up-Left
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 5: // Up-Right
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                            }
                            break;
                        case 6: // Down-Left
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 3: // Right
                                    if (type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 7: // Down-Right
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                            }
                            break;
                        case 8: // Any
                            switch (now)
                            {
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                    }
                    break;
                case 6: // Down-Left
                    switch (before)
                    {
                        case 0: // Up
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 1: // Down
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 2: // Left
                            switch (now)
                            {
                                case 3: // Right
                                    return true;
                                case 7: // Down-Right
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 3: // Right
                            switch (now)
                            {
                                case 2: // Left
                                    if(type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                                case 6: // Down-Left
                                    if(type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 4: // Up-Left
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 5: // Up-Right
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 2: // Left
                                    if(type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                                case 6: // Down-Left
                                    return true;
                            }
                            break;
                        case 6: // Down-Left
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 7: // Down-Right
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                            }
                            break;
                        case 8: // Any
                            switch (now)
                            {
                                case 6: // Down-Left
                                    return true;
                            }
                            break;
                    }
                    break;
                case 7: // Down-Right
                    switch (before)
                    {
                        case 0: // Up
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 1: // Down
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 2: // Left
                            switch (now)
                            {
                                case 3: // Right
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                                case 7: // Down-Right
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 3: // Right
                            switch (now)
                            {
                                case 2: // Left
                                    return true;
                                case 6: // Down-Left
                                    if (type == 1)
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        case 4: // Up-Left
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 3: // right
                                    if (type == 0)
                                    {
                                        return true;
                                    }
                                    break;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 5: // Up-Right
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                            }
                            break;
                        case 6: // Down-Left
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 7: // Down-Right
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                            }
                            break;
                        case 8: // Any
                            switch (now)
                            {
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                    }
                    break;
                case 8: // Any
                    switch (before)
                    {
                        case 0: // Up
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 1: // Down
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 2: // Left
                            switch (now)
                            {
                                case 3: // Right
                                    return true;
                            }
                            break;
                        case 3: // Right
                            switch (now)
                            {
                                case 2: // Left
                                    return true;
                            }
                            break;
                        case 4: // Up-Left
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 7: // Down-Right
                                    return true;
                            }
                            break;
                        case 5: // Up-Right
                            switch (now)
                            {
                                case 1: // Down
                                    return true;
                                case 6: // Down-Left
                                    return true;
                            }
                            break;
                        case 6: // Down-Left
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 5: // Up-Right
                                    return true;
                            }
                            break;
                        case 7: // Down-Right
                            switch (now)
                            {
                                case 0: // Up
                                    return true;
                                case 4: // Up-Left
                                    return true;
                            }
                            break;
                    }
                    break;
            }
                
            return false;
        }

        #endregion

        #region Process for map modification

        void BottomDisplacement()
        {
            Note lastNote = new Note(0, 0, 0, 0, 0);

            foreach (var note in Notes)
            {
                if(note._time - lastNote._time >= -0.01 && note._time - lastNote._time <= 0.01 && note._lineIndex == lastNote._lineIndex)
                {
                    if(note._type == 0 && note._lineIndex != 0)
                    {
                        note._lineIndex--;
                    }
                    else if(note._type == 1 && note._lineIndex != 3)
                    {
                        note._lineIndex++;
                    }
                    else if (note._type == 1)
                    {
                        note._lineIndex--;
                    }
                    else
                    {
                        note._lineIndex++;
                    }
                }

                if (note._lineLayer == 2 || note._lineLayer == 1)
                {
                    note._lineLayer = 0;
                }

                lastNote = note;
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
