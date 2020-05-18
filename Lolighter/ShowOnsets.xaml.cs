using System.Collections.Generic;
using System.Windows;

namespace Lolighter
{
    /// <summary>
    /// Interaction logic for ShowOnsets.xaml
    /// </summary>
    public partial class ShowOnsets : Window
    {
        List<float> peak = new List<float>();
        float avg = 0;

        public ShowOnsets(List<OnsetDetection.Onset> onsets)
        {
            InitializeComponent();

            foreach(var on in onsets)
            {
                peak.Add(on.OnsetAmplitude);
                avg += on.OnsetAmplitude;
            }

            avg /= onsets.Count;

            Average.Content = avg.ToString();
            Count.Content = onsets.Count.ToString();
            ListBox.ItemsSource = peak;
        }
    }
}
