//using Microsoft.Win32;

namespace notepadApplication
{

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

        internal string? SaveTarget
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

        private void mnu_open(object sender, System.Windows.RoutedEventArgs e)
        {

            int action_to_take = -1; // 0 for new document[Yes], 1 for save action [no], -1 for nothing [cancel]

            //check if there is unsaved info
            if (this.SaveState == SaveStatus.Edited)
            {
                //if unsaved info, prompt to: continue anyway [Yes/0]; Save [No/1]; or Cancel [Cancel/-1]
                action_to_take = Confirm_NewDoc();

                //if they chose No, prompt them to save
                if (action_to_take == 1)
                {
                    mnu_save(sender, e);
                }
                else
                {
                    //do nothing as the document remains unsaved
                }
            }

            //if there is nothing unsaved, allow the opening of the new document
            //NB: Unsaved state is the state of a new document which has no content, and thus does not need to be saved before opening a new one
            if (this.SaveState == SaveStatus.Saved || this.SaveState == SaveStatus.Unsaved || action_to_take == 0)
            {
                OpenDocument();
            }
            //user either did not save when prompted, or canceled when asked for confirmation:
            else
            {
                //do nothing
            }
            //return;
            //else open file dialog, and load content from document 

            return; //standard return

        }

        private void mnu_save(object sender, System.Windows.RoutedEventArgs e)
        {
            //if this is not the first save, and there is a save target
            if (this.SaveTarget != null && this.SaveState != SaveStatus.Unsaved)
            {
                //save to the save target and update the savestate to saved and remove asterisk from title
                System.IO.File.WriteAllText(this.SaveTarget, WorkingTextArea.Text);
                this.SaveState = SaveStatus.Saved;
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
                this.SaveState = SaveStatus.Saved;
                this.SaveTarget = saveAsDialogWindow.FileName;
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
                //enable the checkmark and update make-shift alt text
                this.element_mnu_wrap.Header = "_Wrap Text [ON]";
                this.element_mnu_wrap.IsChecked = true;

            }
            else if(this.WorkingTextArea.TextWrapping == System.Windows.TextWrapping.Wrap)
            {
                this.WorkingTextArea.TextWrapping = System.Windows.TextWrapping.NoWrap;
                //disable the checkmark and update make-shift alt text
                this.element_mnu_wrap.Header = "_Wrap Text [OFF]";
                this.element_mnu_wrap.IsChecked = false;
            }
        }


        private void mnu_newDoc(object sender, System.Windows.RoutedEventArgs e)
        {
            int action_to_take = -1; // 0 for new document[Yes], 1 for save action [no], -1 for nothing [cancel]
            
            //check if there is unsaved info
            if(this.SaveState == SaveStatus.Edited)
            {
                //if unsaved info, prompt to: continue anyway [Yes/0]; Save [No/1]; or Cancel [Cancel/-1]
                action_to_take = Confirm_NewDoc();
                
                //if they chose No, prompt them to save
                if (action_to_take == 1)
                {
                    mnu_save(sender, e);
                }
                else
                {
                    //do nothing as the document remains unsaved
                }
            }

            //if there is nothing unsaved, allow the opening of the new document
                //NB: Unsaved state is the state of a new document which has no content, and thus does not need to be saved before opening a new one
            if(this.SaveState == SaveStatus.Saved || this.SaveState == SaveStatus.Unsaved || action_to_take == 0)
            {
                CreateNewDocument();
            }
            //user either did not save when prompted, or canceled when asked for confirmation:
            else
            {
                //do nothing
            }
            return;
        }




        //helper methods
        private int Confirm_NewDoc()
        {
            int action;
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show
                (
                    "The current document has unsaved changes.\n Would you like to discard the changes without saving?",
                    "Warning! Unsaved changes",
                    System.Windows.MessageBoxButton.YesNoCancel,
                    System.Windows.MessageBoxImage.Warning
                );
            //assign an int to the proper action to take
            switch(result)
            {
                case System.Windows.MessageBoxResult.Yes:
                    action = 0;
                    break;
                case System.Windows.MessageBoxResult.No:
                    action = 1;
                    break;
                case System.Windows.MessageBoxResult.Cancel:
                    action = -1;
                    break;
                default:
                    action = -1;
                    break;
            }
            return action;
        }

        //Open a document and load it into the textbox
        void OpenDocument()
        {
            Microsoft.Win32.OpenFileDialog openFileDialogWindow = new Microsoft.Win32.OpenFileDialog();

            //add filters so the dialogue box only shows .txt's and default's the extension to txt
            openFileDialogWindow.Filter = "Text Files (*.txt)|*.txt";

            if (openFileDialogWindow.ShowDialog() == true)
            {
                //write the content of the file to the textbox
                //Personal note for future use: without data scrubbing, this is likely vulnerable to injection
                this.WorkingTextArea.Text = System.IO.File.ReadAllText(openFileDialogWindow.FileName);
                //update the save state, save target and title of the window to remove asterisk if any
                this.SaveState = SaveStatus.Saved;
                this.SaveTarget = openFileDialogWindow.FileName;
                this.Title = this.SaveTarget.ToString();

            }
            else
            {
                //do nothing
            }
        }

        //New document (actually the same txtbox, but resets all the defaults
        //and content instead of opening a new window and using more memory)
        void CreateNewDocument()
        {
            //wipe txtbox;
            this.WorkingTextArea.Clear();
            //set SaveTarget to null;
            this.SaveTarget = null;
            //set to title to Untitled;
            this.Title = "untitled";
            
            //set wrap+checkbox to disabled;
                //Uncomment these lines to have the state of the textwrap reset to nowrap for new documents
            //this.WorkingTextArea.TextWrapping = System.Windows.TextWrapping.NoWrap;
            //this.element_mnu_wrap.Header = "_Wrap Text [OFF]";
            //this.element_mnu_wrap.IsChecked = false;


            //set statusbar text to 0 chars;
            this.StatusText.Text = "Chars: 0";
            //set SaveState to unsaved
            this.SaveState = SaveStatus.Unsaved;
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