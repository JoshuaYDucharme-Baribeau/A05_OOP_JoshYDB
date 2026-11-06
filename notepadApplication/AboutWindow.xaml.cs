/*
 * FILE             : AboutWindow.xaml.cs
 * PROJECT          : Assignment 5
 * PROGRAMMER       : Joshua Ducharme-Baribeau
 * FIRST VERSION    : November 8 2026
 * DESCRIPTION      : C# Source file for the AboutWindow partial class.
 *                    
 *                    
 *                    
 */
namespace notepadApplication
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : System.Windows.Window
    {
        //constructor
        public AboutWindow()
        {
            InitializeComponent();
        }

        /*
         *	METHOD  	: btn_close_about
         *	DESCRIPTION	: method that closes the about window
         *		
         *	
         *	PARAMETERS	: object sender : the object that triggered the event
         *                System.Windows.RoutedEventArgs e : arguments passed by the sender	
         *	
         *	
         *	RETURNS		: VOID
         */
        private void btn_close_about(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
