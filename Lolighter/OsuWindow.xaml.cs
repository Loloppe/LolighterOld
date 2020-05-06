
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Lolighter.Items;
using Osu2Saber.Model;

namespace Lolighter
{
    /// <summary>
    /// Interaction logic for OsuWindow.xaml
    /// </summary>
    public partial class OsuWindow : Window
    {
        public string workDir = AppDomain.CurrentDomain.BaseDirectory;
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

            _Notes Note = null;
            List<_Notes> Notes = new List<_Notes>();

            foreach (var x in Osu2BsConverter.map._notes)
            {
                Note = new _Notes(x._time, x._lineIndex, x._lineLayer, x._type, x._cutDirection);
                Notes.Add(Note);
            }

            MainWindow.map._notes = Notes.ToArray();

            MainWindow.IsReady();
            Close();
        }
    }
}
