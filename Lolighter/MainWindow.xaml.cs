using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Lolighter.Items;
using Lolighter.Methods;
using System.Globalization;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using Osu2Saber.ViewModel;

namespace Lolighter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Rootobject map = new Rootobject();
        public static MainWindow window;
        private String extension;

        public MainWindow()
        {
            InitializeComponent();
            window = this;
        }

        #region File

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog
            {
                Filter = "dat osz zip|*.dat;*.osz;*.zip",
                Multiselect = false
            };

            if (open.ShowDialog() == true)
            {
                extension = Path.GetExtension(open.FileName);
                String file = open.FileNames[0];

                if (extension == ".dat")
                {
                    var mapFile = new MapFile(file);
                    string data = File.ReadAllText(mapFile.Path);
                    map = JsonConvert.DeserializeObject<Rootobject>(data);
                }
                else if (extension == ".osz" || extension == ".zip")
                {
                    OsuWindow o = new OsuWindow();
                    o.OszFiles.Clear();
                    o.Show();
                    o.OszFiles.Add(file);
                }

                try
                {
                    if (extension == ".dat" && map._notes != null)
                    {
                        IsReady();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error: No note available");
                    map = null;
                }
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = FileName.Text+".dat";

                using (StreamWriter wr = new StreamWriter(path))
                {
                    wr.WriteLine(JsonConvert.SerializeObject(map));
                }

                MessageBox.Show("A new file has been created");

                ProcessStartInfo dir = new ProcessStartInfo
                {
                    Arguments = AppDomain.CurrentDomain.BaseDirectory,
                    FileName = "explorer.exe"
                };

                Process.Start(dir);

                map = null;
                map = new Rootobject();

                SimpleLighter.IsEnabled = false;
                SaveFile.IsEnabled = false;
                SlidersMadness.IsEnabled = false;
                InvertedMadness.IsEnabled = false;
                BombGenerator.IsEnabled = false;
                LoloppeGenerator.IsEnabled = false;
                Downscale.IsEnabled = false;
                OpenFile.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void IsReady()
        {
            window.SaveFile.IsEnabled = true;
            window.SimpleLighter.IsEnabled = true;
            window.SlidersMadness.IsEnabled = true;
            window.InvertedMadness.IsEnabled = true;
            window.BombGenerator.IsEnabled = true;
            window.LoloppeGenerator.IsEnabled = true;
            window.Downscale.IsEnabled = true;
            window.OpenFile.IsEnabled = false;
        }

        #endregion

        #region Modification

        private void SlidersMadness_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes> noteTemp = new List<_Notes>(map._notes);
            map._notes = null;

            noteTemp = Sliders.MakeSliders(noteTemp, Convert.ToDouble(Limiter.Text, CultureInfo.InvariantCulture), AllowLimiter.IsChecked.GetValueOrDefault());

            map._notes = noteTemp.ToArray();

            SlidersMadness.IsEnabled = false;
        }
        
        private void SimpleLighter_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes> noteTempo = null;

            noteTempo = new List<_Notes>(map._notes);
            noteTempo = noteTempo.OrderBy(a => a._time).ToList();
            if (map._events != null)
            {
                map._events = null;
            }

            List<_Events> eventTempo = Light.CreateLight(noteTempo, Convert.ToDouble(ColorOffset.Text, CultureInfo.InvariantCulture), Convert.ToDouble(ColorSwapSpeed.Text, CultureInfo.InvariantCulture),
                AllowBackStrobe.IsChecked.GetValueOrDefault(), AllowNeonStrobe.IsChecked.GetValueOrDefault(), AllowSideStrobe.IsChecked.GetValueOrDefault(), AllowFade.IsChecked.GetValueOrDefault(), AllowSpin.IsChecked.GetValueOrDefault(),
                Convert.ToInt16(SlowMinSpinSpeed.Text), Convert.ToInt16(SlowMaxSpinSpeed.Text), Convert.ToInt16(FastMinSpinSpeed.Text), Convert.ToInt16(FastMaxSpinSpeed.Text));

            List<_Events> sorted = eventTempo.OrderBy(o => o._time).ToList();

            map._events = sorted.ToArray();

            SimpleLighter.IsEnabled = false;
        }

        private void InvertedMadness_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes>  noteTemp = new List<_Notes>(map._notes);
            map._notes = null;

            noteTemp = Inverted.MakeInverted(noteTemp, Convert.ToDouble(Limiter.Text, CultureInfo.InvariantCulture), AllowLimiter.IsChecked.GetValueOrDefault());

            map._notes = noteTemp.ToArray();

            InvertedMadness.IsEnabled = false;
        }

        private void BombGenerator_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes>  noteTemp = new List<_Notes>(map._notes);
            map._notes = null;

            noteTemp = Bomb.CreateBomb(noteTemp);

            map._notes = noteTemp.ToArray();

            BombGenerator.IsEnabled = false;
        }

        private void LoloppeGenerator_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes> noteTemp = new List<_Notes>(map._notes);
            map._notes = null;

            noteTemp = Loloppe.LoloppeGen(noteTemp);

            map._notes = noteTemp.ToArray();

            LoloppeGenerator.IsEnabled = false;
        }

        private void Downscaler_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes> noteTemp = new List<_Notes>(map._notes);
            map._notes = null;

            noteTemp = Downscaler.Downscale(noteTemp);

            map._notes = noteTemp.ToArray();

            Downscale.IsEnabled = false;
        }
        #endregion

        public class MapFile
        {
            public string Path { get; private set; }

            public double? StartTimeInMS = null;

            public MapFile(string selectedFile)
            {
                this.Path = selectedFile;
            }
        }

    }
}
