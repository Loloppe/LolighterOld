
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public OsuWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            await Task.Run(() => MainWindow.Osu2BS.ProcessBatch());
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
