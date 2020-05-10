
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Lolighter.Items;
using Newtonsoft.Json;
using Osu2Saber.Model;
using Osu2Saber.Model.Algorithm;
using Osu2Saber.Model.Json;

namespace Lolighter
{
    /// <summary>
    /// Interaction logic for OsuWindow.xaml
    /// </summary>
    public partial class OsuWindow : Window
    {
        public static Rootobject map = new Rootobject();
        public string workDir = AppDomain.CurrentDomain.BaseDirectory;
        private String extension;
        public ObservableCollection<string> OszFiles { get; } = new ObservableCollection<string>();

        public OsuWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            BatchProcessor bp = new BatchProcessor(OszFiles.ToArray(), workDir);
            await bp.BatchProcess();
            MessageBox.Show("Finished parsing notes");
            List<Items._Notes> Notes = new List<Items._Notes>();

            foreach (var x in Osu2BsConverter.map._notes)
            {
                Items._Notes Note = new Items._Notes(x._time, x._lineIndex, x._lineLayer, x._type, x._cutDirection);
                Notes.Add(Note);
            }

            MainWindow.map._notes = Notes.ToArray();

            MainWindow.IsReady();
            Close();
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
    }
}
