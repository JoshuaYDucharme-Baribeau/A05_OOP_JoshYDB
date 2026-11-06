

namespace notepadApplication
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : System.Windows.Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void btn_close_about(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
