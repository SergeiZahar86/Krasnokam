using System.Windows;

namespace Krasnokam.DialogWindows
{
    public partial class VerificationDeleetStack : Window
    {
        private Global global;

        public VerificationDeleetStack()
        {
            global = Global.getInstance();
            InitializeComponent();
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            global.deleetBorder = true;
            this.Close();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
