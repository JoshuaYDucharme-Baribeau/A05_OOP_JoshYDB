//using Microsoft.Win32;


//personal note for future development: consider refactoring the confirmation dialogs, but the standard clickevents to trigger messageBox does not allow passing of additional arguments without some workarounds


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
            //event handlers


        private void mnu_about(object sender, System.Windows.RoutedEventArgs e)
        {
            AboutWindow abt_window = new AboutWindow();
            abt_window.ShowDialog();
        }

        //* triggers when selecting wrap text from window's menu
        private void mnu_WrapText(object sender, System.Windows.RoutedEventArgs e)
        {
            ToggleWrapOption();
        }

        //only handles the event so that the actual logic for saving can be used by other events
        private void mnu_save(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveDocument();
        }

        private void mnu_saveAs(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveDocumentAs();
        }

        private void mnu_newDoc(object sender, System.Windows.RoutedEventArgs e)
        {
            DetermineActionAndPrompt("new");
        }


        private void mnu_open(object sender, System.Windows.RoutedEventArgs e)
        {
            DetermineActionAndPrompt("open");
        }

        //Exit menu item
        private void mnu_exit_app(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        //Changes the standard "X" window button to not close the window automatically.
        //Instead, it calls the mnu_exit method so that the behaviour is identical (as per assignment instructions)
        //this prrovides the user the opportunity to save unsaved changes
        //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/windows/how-to-close-window-dialog-box
        //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/windows/#cancel-window-closure
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //instead of closing
            //determine action to take, pass "exit"
            if(DetermineActionAndPrompt("exit") == -1)
            {
                e.Cancel = true;
            }
            else
            {
                //do nothing to interrupt, app will close
            }
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



        //Methods
            //logic and helpers
        
        //making this reusable by the other menu item logic
        private int ConfirmUserAction(string message, string caption)
        {
            int action;
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show
                (
                    message,
                    caption,
                    System.Windows.MessageBoxButton.YesNoCancel,
                    System.Windows.MessageBoxImage.Warning
                );
            //assign an int to the proper action to take
            switch (result)
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

        private int DetermineActionAndPrompt(string action)
        {
            int result = 0;
            int prompt_result = -1; // 0 for new document[Yes], 1 for save action [no], -1 for nothing [cancel]

            string prompt_message;
            string notice_message;

            switch (action)
            {
                case "open":
                    prompt_message = "The current document has unsaved changes.\nWould you like to save the changes before opening a new document?";
                    notice_message = "Select a document to open.";
                    break;

                case "new":
                    prompt_message = "The current document has unsaved changes.\nWould you like to save the changes before creating a new document?";
                    notice_message = "A new document will be created.";
                    break;
                case "exit":
                    prompt_message = "The current document has unsaved changes.\nWould you like to save the changes before exiting the application?";
                    notice_message = "The application will now close.";

                    break;
                default:
                    //should not occur
                    prompt_message = "The current document has unsaved changes.";
                    notice_message = "The process may not have been completed successfully.";
                    break;
            }

            //check if there is unsaved info
            if (this.SaveState == SaveStatus.Edited)
            {
                //if unsaved info, prompt to: continue anyway [Yes/0]; Save [No/1]; or Cancel [Cancel/-1]
                prompt_result = ConfirmUserAction(prompt_message, "Warning! Unsaved changes");

                //if they chose Yes, prompt them to save
                if (prompt_result == 0)
                {
                    SaveDocument();
                    if (this.SaveState == SaveStatus.Saved)
                    {
                        //dialogue box to confirm save and advise the user a new dialogue box will open
                        System.Windows.MessageBox.Show("The current document has been saved. " + notice_message, "Notice", System.Windows.MessageBoxButton.OK);
                    }
                }
                else
                {
                    //do nothing as the document remains unsaved
                }
            }

            //if there is nothing unsaved or if the document has been saved since the user was prompted
            //NB: Unsaved state is the state of a new document which has no content, and thus does not need to be proceeding
            if (this.SaveState == SaveStatus.Saved || this.SaveState == SaveStatus.Unsaved || prompt_result == 1)
            {
                switch (action)
                {
                    case "open":
                        OpenDocument();
                        break;
                    case "new":
                        ResetWorkArea();
                        break;
                    case "exit":
                        result = 0; //ensure the application returns a 0 which will not cancel the exit
                        break;
                    default:
                        break;
                }
            }
            //user either did not save when prompted, or canceled when asked for confirmation:
            else
            {
                result = -1; // return a -1 to indicate the operation was cancelled
            }

            return result;
        }


        //save logic
        private void SaveDocument()
        {
            //if this is not the first save, and there is a save target
            if (this.SaveTarget != null && this.SaveState != SaveStatus.Unsaved)
            {
                //save to the save target and update the savestate to saved and remove asterisk from title
                System.IO.File.WriteAllText(this.SaveTarget, this.WorkingTextArea.Text);
                this.SaveState = SaveStatus.Saved;
                this.Title = this.SaveTarget.ToString();
            }
            //if there is no save target or the text has never been saved before
            else
            {
                //open a SaveAs dialog by passing the arguments along
                SaveDocumentAs();
            }
            return; //standard return
        }

        private void SaveDocumentAs()
        {
            Microsoft.Win32.SaveFileDialog saveAsDialogWindow = new Microsoft.Win32.SaveFileDialog();

            //add filters so the dialogue box only shows .txt's and default's the extension to txt
            saveAsDialogWindow.Filter = "Text Files (*.txt)|*.txt";

            if (saveAsDialogWindow.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveAsDialogWindow.FileName, this.WorkingTextArea.Text);
                //update the save state, save target and title of the window to remove asterisk if any
                this.SaveState = SaveStatus.Saved;
                this.SaveTarget = saveAsDialogWindow.FileName;
                this.Title = this.SaveTarget.ToString();

            }
            return; //standard return
        }

        //New document (actually the same txtbox, but resets all the defaults
        //and content instead of opening a new window and using more memory)
        private void ResetWorkArea()
        {
            //wipe txtbox; set SaveTarget to null; set to title to Untitled;
            this.WorkingTextArea.Clear();
            this.SaveTarget = null;
            this.Title = "untitled";

            //set statusbar text to 0 chars; set SaveState to unsaved
            this.StatusText.Text = "Chars: 0";
            this.SaveState = SaveStatus.Unsaved;

            //set wrap+checkbox to disabled;
                //Uncomment these lines to have the state of the textwrap reset to nowrap for new documents
            //this.WorkingTextArea.TextWrapping = System.Windows.TextWrapping.NoWrap;
            //this.element_mnu_wrap.Header = "_Wrap Text [OFF]";
            //this.element_mnu_wrap.IsChecked = false;
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

        //toggle logic for wrapping text
        private void ToggleWrapOption()
        {
            if (this.WorkingTextArea.TextWrapping == System.Windows.TextWrapping.NoWrap)
            {
                this.WorkingTextArea.TextWrapping = System.Windows.TextWrapping.Wrap;
                //enable the checkmark and update make-shift alt text
                this.element_mnu_wrap.Header = "_Wrap Text [ON]";
                this.element_mnu_wrap.IsChecked = true;

            }
            else if (this.WorkingTextArea.TextWrapping == System.Windows.TextWrapping.Wrap)
            {
                this.WorkingTextArea.TextWrapping = System.Windows.TextWrapping.NoWrap;
                //disable the checkmark and update make-shift alt text
                this.element_mnu_wrap.Header = "_Wrap Text [OFF]";
                this.element_mnu_wrap.IsChecked = false;
            }
        }

        //make an enum for the various save statuses of the document
        internal enum SaveStatus
        {
            Unsaved,
            Saved,
            Edited
        }


    }
}