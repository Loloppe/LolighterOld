using System.Windows;

namespace Osu2Saber.Model
{
    /// <summary>
    /// Interaction logic for PromptDialog.xaml
    /// </summary>
    public partial class PromptDialog : Window
    {
        public PromptDialog(string question, string title, string defaultValue = "")
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PromptDialog_Loaded);
            txtQuestion.Text = question;
            Title = title;
            txtResponse.Text = defaultValue;
        }

        void PromptDialog_Loaded(object sender, RoutedEventArgs e)
        {
                txtResponse.Focus();
        }

        public static string Prompt(string question, string title, string defaultValue = "")
        {
            PromptDialog inst = new PromptDialog(question, title, defaultValue);
            inst.ShowDialog();
            if (inst.DialogResult == true)
                return inst.ResponseText;
            return null;
        }

        public string ResponseText
        {
            get
            {
                 return txtResponse.Text;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
