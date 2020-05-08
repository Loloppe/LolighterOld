using System.Windows;
using System.Windows.Controls;

namespace Osu2Saber.View
{
    /// <summary>
    /// ConfigPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class ConfigPanel : UserControl
    {
        public ConfigPanel()
        {
            InitializeComponent();
        }

        private void Pattern_Click(object sender, RoutedEventArgs e)
        {
            PatternWindow pc = new PatternWindow();
            pc.Show();
        }
    }
}
