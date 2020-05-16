
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Lolighter.Items;
using Newtonsoft.Json;
using OnsetDetection;
using Osu2Saber.Model;
using Osu2Saber.Model.Algorithm;
using Osu2Saber.Model.Json;
using Enum = System.Enum;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Lolighter
{
    /// <summary>
    /// Interaction logic for OsuWindow.xaml
    /// </summary>
    public partial class ConverterWindow : Window
    {
        public static Rootobject map = new Rootobject();
        public string workDir = AppDomain.CurrentDomain.BaseDirectory;
        private String extension;
        public ObservableCollection<string> OszFiles { get; } = new ObservableCollection<string>();
        public static ConverterWindow window;

        public ConverterWindow()
        {
            InitializeComponent();

            Combo.ItemsSource = Enum.GetValues(typeof(Detectors));
            Combo.SelectedIndex = 1;
            window = this;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OszFiles.Count() == 0)
            {
                MessageBox.Show("You must open a .osz file first");
            }
            else
            {
                IsEnabled = false;

                BatchProcessor bp = new BatchProcessor(OszFiles.ToArray(), workDir);
                await bp.BatchProcess();
                MessageBox.Show("Finished parsing notes");
                List<Items._Notes> Notes = new List<Items._Notes>();

                if (Osu2BsConverter.map._notes != null)
                {
                    foreach (var x in Osu2BsConverter.map._notes)
                    {
                        Items._Notes Note = new Items._Notes(x._time, x._lineIndex, x._lineLayer, x._type, x._cutDirection);
                        Notes.Add(Note);
                    }
                }

                MainWindow.map._notes = Notes.ToArray();

                MainWindow.IsReady();
                Close();
            }
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "dat|*.dat",
                Multiselect = false,
                InitialDirectory = workDir
            };

            if (open.ShowDialog() == true)
            {
                extension = Path.GetExtension(open.FileName);
                String file = open.FileNames[0];
                if (extension == ".dat")
                {
                    var mapFile = new MainWindow.MapFile(file);
                    string data = File.ReadAllText(mapFile.Path);
                    // Ask for BPM
                    SetBPM();
                    map = JsonConvert.DeserializeObject<Rootobject>(data);
                    List<Note> n = new List<Note>();
                    foreach (var no in map._notes)
                    {
                        Note not = new Note(no._time, no._lineIndex, no._lineLayer, no._type, no._cutDirection);
                        n.Add(not);
                    }
                    var ca = new ConvertAlgorithm(n);
                    ca.ConvertDat();

                    List<Items._Notes> Notes = new List<Items._Notes>();
                    foreach (var x in ca.Notes)
                    {
                        Items._Notes Note = new Items._Notes(x._time, x._lineIndex, x._lineLayer, x._type, x._cutDirection);
                        Notes.Add(Note);
                    }

                    MainWindow.map._notes = Notes.ToArray();

                    MainWindow.IsReady();
                    Close();
                }
            }
        }

        private void Audio_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "mp3 wav flac|*.mp3;*wav;*flac",
                Multiselect = false,
                InitialDirectory = workDir
            };

            if (open.ShowDialog() == true)
            {
                extension = Path.GetExtension(open.FileName);
                String file = open.FileNames[0];
                // Ask for BPM
                SetBPM();
                // We get the onsets here
                List<double> time = new List<double>(TestRobustness(file));
                
                map = new Rootobject();
                List<Note> n = new List<Note>();
                foreach (var t in time)
                {
                    Note not = new Note(t, 0, 0, 0, 0);
                    n.Add(not);
                }
                var ca = new ConvertAlgorithm(n);
                ca.ConvertDat();

                List<Items._Notes> Notes = new List<Items._Notes>();
                foreach (var x in ca.Notes)
                {
                    Items._Notes Note = new Items._Notes(x._time, x._lineIndex, x._lineLayer, x._type, x._cutDirection);
                    Notes.Add(Note);
                }

                MainWindow.map._notes = Notes.ToArray();

                MainWindow.IsReady();
                Close();
            }
        }

        public static void SetBPM()
        {
            var pd = PromptDialog.Prompt("Enter the BPM", "BPM");
            if (pd != null)
            {
                ConvertAlgorithm.setBPM = double.Parse(pd);
            }
        }

        public static List<double> TestRobustness(string f)
        {
            MessageBox.Show("This might take a bit");
            var options = DetectorOptions.Default;
            options.ActivationThreshold = float.Parse(window.TBox.Text);
            options.SliceLength = 10.0f;
            options.SlicePaddingLength = 0.5f;
            options.Online = false;
            options.DetectionFunction = (Detectors)window.Combo.SelectedItem;
            options.AdaptiveWhitening = window.CBox.IsChecked.Value;
            var onsetDetector = new OnsetDetector(options, null);
            var onsets = onsetDetector.Detect(f);
            List<double> time = new List<double>();
            foreach(var o in onsets)
            {
                // Skip those too close to the start.
                if(o.OnsetTime < 1)
                {
                    continue;
                }
                // Sec to MS
                double t = o.OnsetTime * 1000;
                // 60000 / BPM = MSperBeat
                time.Add(t / (60000 / ConvertAlgorithm.setBPM) + 0.1);
                // We add 0.1 to fix offset caused by the algorithm.
                if(o.OnsetAmplitude > float.Parse(window.TboxIntensity.Text))
                {
                    time.Add(t / (60000 / ConvertAlgorithm.setBPM) + 0.1);
                }
            }
            return time;
        }
    }
}
