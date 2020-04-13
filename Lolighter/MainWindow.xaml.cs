using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using Lolighter.Items;
using Lolighter.Methods;
using System.Globalization;

namespace Lolighter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Rootobject map;
        private String extension;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        #region File

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog
            {
                Filter = "Dat|*.dat",
                Multiselect = true
            };

            if (open.ShowDialog() == true)
            {
                extension = System.IO.Path.GetExtension(open.FileName);
                foreach (String file in open.FileNames)
                {
                    var mapFile = new MapFile(file);
                    string jsonData = File.ReadAllText(mapFile.Path);
                    if(extension == ".dat")
                    {
                        map = JsonConvert.DeserializeObject<Rootobject>(jsonData);
                    }
                }
                try
                {
                    if (extension == ".dat")
                    {
                       if(map._notes != null)
                       {
                            SaveFile.IsEnabled = true;
                            SimpleLighter.IsEnabled = true;
                            SlidersMadness.IsEnabled = true;
                            InvertedMadness.IsEnabled = true;
                            BombGenerator.IsEnabled = true;
                            LoloppeGenerator.IsEnabled = true;
                            Downscale.IsEnabled = true;
                            OpenFile.IsEnabled = false;
                        }
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
                string path = FileName.Text+extension;
                using (StreamWriter wr = new StreamWriter(path))
                    if(extension == ".dat")
                    {
                        wr.WriteLine(JsonConvert.SerializeObject(map));
                    }

                MessageBox.Show("A new file has been created");
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

        #endregion

        #region Modification

        private void SlidersMadness_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes> noteTemp = new List<_Notes>();

            if (extension == ".dat")
            {
                noteTemp = new List<_Notes>(map._notes);
                map._notes = null;
            }

            noteTemp = Sliders.MakeSliders(noteTemp, Convert.ToDouble(Limiter.Text, CultureInfo.InvariantCulture), AllowLimiter.IsChecked.GetValueOrDefault());

            if (extension == ".dat")
            {
                map._notes = noteTemp.ToArray();
            }

            SlidersMadness.IsEnabled = false;
        }
        
        private void SimpleLighter_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes> noteTempo = null;

            if (extension == ".dat")
            {
                noteTempo = new List<_Notes>(map._notes);
                noteTempo = noteTempo.OrderBy(a => a._time).ToList();
                if (map._events != null)
                {
                    map._events = null;
                }
            }

            List<_Events> eventTempo = Light.CreateLight(noteTempo, Convert.ToDouble(ColorOffset.Text, CultureInfo.InvariantCulture), Convert.ToDouble(ColorSwapSpeed.Text, CultureInfo.InvariantCulture),
                AllowStrobe.IsChecked.GetValueOrDefault(), AllowFade.IsChecked.GetValueOrDefault(), AllowSpin.IsChecked.GetValueOrDefault(),
                Convert.ToInt16(SlowMinSpinSpeed.Text), Convert.ToInt16(SlowMaxSpinSpeed.Text), Convert.ToInt16(FastMinSpinSpeed.Text), Convert.ToInt16(FastMaxSpinSpeed.Text));

            List<_Events> sorted = eventTempo.OrderBy(o => o._time).ToList();
            
            if (extension == ".dat")
            {
                map._events = sorted.ToArray();
            }

            SimpleLighter.IsEnabled = false;
        }

        private void InvertedMadness_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes> noteTemp = new List<_Notes>();

            if (extension == ".dat")
            {
                noteTemp = new List<_Notes>(map._notes);
                map._notes = null;
            }

            noteTemp = Inverted.MakeInverted(noteTemp, Convert.ToDouble(Limiter.Text, CultureInfo.InvariantCulture), AllowLimiter.IsChecked.GetValueOrDefault());

            if (extension == ".dat")
            {
                map._notes = noteTemp.ToArray();
            }

            InvertedMadness.IsEnabled = false;
        }

        private void BombGenerator_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes> noteTemp = new List<_Notes>();

            if (extension == ".dat")
            {
                noteTemp = new List<_Notes>(map._notes);
                map._notes = null;
            }

            noteTemp = Bomb.CreateBomb(noteTemp);

            if (extension == ".dat")
            {
                map._notes = noteTemp.ToArray();
            }

            BombGenerator.IsEnabled = false;
        }

        private void LoloppeGenerator_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes> noteTemp = new List<_Notes>();
            

            if (extension == ".dat")
            {
                noteTemp = new List<_Notes>(map._notes);
                map._notes = null;
            }

            noteTemp = Loloppe.LoloppeGen(noteTemp);

            if (extension == ".dat")
            {
                map._notes = noteTemp.ToArray();
            }

            LoloppeGenerator.IsEnabled = false;
        }

        private void Downscaler_Click(object sender, RoutedEventArgs e)
        {
            List<_Notes> noteTemp = new List<_Notes>();

            if (extension == ".dat")
            {
                noteTemp = new List<_Notes>(map._notes);
                map._notes = null;
            }

            noteTemp = Downscaler.Downscale(noteTemp);

            if (extension == ".dat")
            {
                map._notes = noteTemp.ToArray();
            }

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
