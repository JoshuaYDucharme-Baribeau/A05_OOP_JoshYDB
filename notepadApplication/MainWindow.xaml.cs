
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

        //establish properties
        private SaveStatus _saveState;
        private string? _saveTarget = null;

        internal SaveStatus SaveState
        {
            get {  return _saveState; }
            set { _saveState = value; }
        }


        internal string SaveTarget
        {
            get { return _saveTarget; }
            set { _saveTarget = value; }
        }

        //methods
        private void mnu_about(object sender, System.Windows.RoutedEventArgs e)
        {
            AboutWindow abt_window = new AboutWindow();
            abt_window.ShowDialog();
        }

        private void mnu_save(object sender, System.Windows.RoutedEventArgs e)
        {
            //if this is not the first save, and there is a save target
            if (this._saveTarget != null && this.SaveState != SaveStatus.Unsaved)
            {
                //save to the save target and update the savestate to saved and remove asterisk from title
                System.IO.File.WriteAllText(this._saveTarget, WorkingTextArea.Text);
                this._saveState = SaveStatus.Saved;
                this.Title = this.SaveTarget.ToString();
            }
            //if there is no save target or the text has never been saved before
            else
            {
                //open a SaveAs dialog by passing the arguments along
                mnu_saveAs(sender, e);
            }
            return; //standard return
        }

        private void mnu_saveAs(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveAsDialogWindow = new Microsoft.Win32.SaveFileDialog();

            //add filters so the dialogue box only shows .txt's and default's the extension to txt
            saveAsDialogWindow.Filter = "Text Files (*.txt)|*.txt";

            if(saveAsDialogWindow.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveAsDialogWindow.FileName, WorkingTextArea.Text);
                //update the save state, save target and title of the window to remove asterisk if any
                this._saveState = SaveStatus.Saved;
                this._saveTarget = saveAsDialogWindow.FileName;
                this.Title = this.SaveTarget.ToString();

            }
            return; //standard return
        }

            //triggers when the textbox's text is changed by the user
        private void WorkArea_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //only triggers once per save
            if (this.SaveState == SaveStatus.Unsaved || this.SaveState == SaveStatus.Saved)
            {
                //update the file name to add an asterisk
                this.Title = this.Title + "*";
                //change savestate to edited
                this.SaveState = SaveStatus.Edited;
            }

            //update the statusbar
            this.StatusText.Text = "Chars: " + this.WorkingTextArea.Text.Length.ToString();
            //can also add lines and other details to this status bar if desired

        }

        //* triggers when selecting wrap text from window's menu
        private void mnu_WrapText(object sender, System.Windows.RoutedEventArgs e)
        {

            if(this.WorkingTextArea.TextWrapping == System.Windows.TextWrapping.NoWrap)
            {
                this.WorkingTextArea.TextWrapping = System.Windows.TextWrapping.Wrap;
                this.element_mnu_wrap.Header = "_Wrap Text [ON]";

            }
            else if(this.WorkingTextArea.TextWrapping == System.Windows.TextWrapping.Wrap)
            {
                this.WorkingTextArea.TextWrapping = System.Windows.TextWrapping.NoWrap;
                this.element_mnu_wrap.Header = "_Wrap Text [OFF]";
            }
        }


        //make an enum for the savestatus of the document
        internal enum SaveStatus
        {
            Unsaved,
            Saved,
            Edited
        }
    }
}