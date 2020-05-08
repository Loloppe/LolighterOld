using Microsoft.Win32;
using Newtonsoft.Json;
using Osu2Saber.Model;
using Osu2Saber.Model.Algorithm;
using Osu2Saber.Model.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace Osu2Saber.View
{
    public partial class PatternControl : UserControl
    {
        Pack pack;
        List<_Pattern> _patterns = new List<_Pattern>();

        public ObservableCollection<string> PatternsName { get; set; }

        List<_Notes> _notes = new List<_Notes>();

        public PatternControl()
        {
            InitializeComponent();

            PatternsName = new ObservableCollection<string>();
            Type.Items.Add(NoteType.Red);
            Type.Items.Add(NoteType.Blue);
            Type.SelectedIndex = 0;
            Lane.Items.Add(Line.Left);
            Lane.Items.Add(Line.MiddleLeft);
            Lane.Items.Add(Line.MiddleRight);
            Lane.Items.Add(Line.Right);
            Lane.SelectedIndex = 0;
            Lay.Items.Add(Layer.Bottom);
            Lay.Items.Add(Layer.Middle);
            Lay.Items.Add(Layer.Top);
            Lay.SelectedIndex = 0;
            Cut.Items.Add(CutDirection.Up);
            Cut.Items.Add(CutDirection.Down);
            Cut.Items.Add(CutDirection.Left);
            Cut.Items.Add(CutDirection.Right);
            Cut.Items.Add(CutDirection.UpLeft);
            Cut.Items.Add(CutDirection.UpRight);
            Cut.Items.Add(CutDirection.DownLeft);
            Cut.Items.Add(CutDirection.DownRight);
            Cut.Items.Add(CutDirection.Any);
            Cut.SelectedIndex = 0;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Add the new note to the pattern and to the list.
            if(Lane.SelectedIndex != -1 && Lay.SelectedIndex != -1 && Type.SelectedIndex != -1 && Cut.SelectedIndex != -1 && Pattern.SelectedIndex != -1)
            {
                _Notes n = new _Notes(0, Lane.SelectedIndex, Lay.SelectedIndex, Type.SelectedIndex, Cut.SelectedIndex);
                _notes.Add(n);
                Notes.Items.Add(Notes.Items.Count.ToString() + " | " + Type.SelectedItem + " | " + Lane.SelectedItem + " | " + Lay.SelectedItem + " | " + Cut.SelectedItem);
                _patterns[Pattern.SelectedIndex]._notes = _notes.ToArray();
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if(Notes.SelectedIndex != -1 && Pattern.SelectedIndex != -1)
            {
                // Remove a note from list and pattern.
                _notes.RemoveAt(Notes.SelectedIndex);
                Notes.Items.Remove(Notes.SelectedItem);
                _patterns[Pattern.SelectedIndex]._notes = _notes.ToArray();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Save the current pack as file
            if (currentPack.Content.ToString() != "")
            {
                pack._pattern = _patterns.ToArray();
                var sz = JsonConvert.SerializeObject(pack);
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\"+ currentPack.Content + ".pak", sz);
                PatternWindow.window.Close();
            }
            else
            {
                MessageBox.Show("Error, packName is null");
            }
        }

        private void Pattern_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Pattern.SelectedIndex != -1)
            {
                if (_patterns[Pattern.SelectedIndex]._notes.Length != 0)
                {
                    _notes.Clear();
                    Notes.Items.Clear();
                    foreach (var note in _patterns[Pattern.SelectedIndex]._notes)
                    {
                        // When changing pattern, need to reload all notes.
                        _notes.Add(note);
                        Notes.Items.Add(Notes.Items.Count.ToString() + " | " + ((NoteType)note._type).ToString() + " | " + ((Line)note._lineIndex).ToString() + " | " + ((Layer)note._lineLayer).ToString() + " | " + ((CutDirection)note._cutDirection).ToString());
                    }
                }
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            // Create a new pack
            var a = PromptDialog.Prompt("Pack Name", "Pack");
            if (a != null)
            {
                pack = new Pack();
                currentPack.Content = a;
                _patterns.Clear();
                PatternsName.Clear();
                Pattern.ItemsSource = null;
                Pattern.ItemsSource = PatternsName;
            }
            else
            {
                MessageBox.Show("Error, string is null.");
            }
        }

        private void NewPat_Click(object sender, RoutedEventArgs e)
        {
            // Add a new pattern to pack
            var a = PromptDialog.Prompt("Pattern Name", "Pattern");
            if (a != null && currentPack.Content.ToString() != "")
            {
                _Pattern pat = new _Pattern
                {
                    name = a,
                    _notes = new _Notes[0]
                };
                _patterns.Add(pat);
                PatternsName.Add(a);
                Pattern.ItemsSource = null;
                Pattern.ItemsSource = PatternsName;
            }
        }

        private void DelPat_Click(object sender, RoutedEventArgs e)
        {
            // Delete pattern from pack
            if(Pattern.SelectedIndex != -1)
            {
                if (PatternsName.Count() > 0)
                {
                    _patterns.RemoveAt(Pattern.SelectedIndex);
                    PatternsName.RemoveAt(Pattern.SelectedIndex);
                    Pattern.ItemsSource = null;
                    Pattern.ItemsSource = PatternsName;
                }
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Open pack from file

            OpenFileDialog open = new OpenFileDialog
            {
                Filter = "pak|*.pak",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Multiselect = false
            };

            if (open.ShowDialog() == true)
            {
                string data = File.ReadAllText(open.FileName);
                pack = JsonConvert.DeserializeObject<Pack>(data);
                currentPack.Content = Path.GetFileNameWithoutExtension(open.FileName);
                _patterns.Clear();
                _patterns.AddRange(pack._pattern);
                PatternsName.Clear();
                foreach (var x in _patterns)
                {
                    PatternsName.Add(x.name);
                }
                Pattern.ItemsSource = null;
                Pattern.ItemsSource = PatternsName;
            }
        }
    }
}
