

namespace notepadApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void mnu_about(object sender, System.Windows.RoutedEventArgs e)
        {
            AboutWindow abt_window = new AboutWindow();
            abt_window.ShowDialog();
        }
    }
}