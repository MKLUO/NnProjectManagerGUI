using Microsoft.Win32;
using System.IO;

using System.Windows;

using Microsoft.WindowsAPICodePack.Dialogs;


namespace NnManagerGUI.ViewModel {
    static class UtilGUI {

        public static string? OpenInputDialogToGetText(string question, string defaultAnswer) {
            var dialog = new View.Dialogs.InputDialog(question, defaultAnswer);
            if (dialog.ShowDialog() == true) 
                if (dialog.DialogResult == true)
                    return dialog.Answer;

            return null;
        }

        static string? LastPath { get; set; }  = null;
        public static string? OpenFileDialogToGetFolder() {
            using CommonOpenFileDialog dialog = new CommonOpenFileDialog {
                InitialDirectory = LastPath ?? "C:\\Users",
                IsFolderPicker = true,
                EnsureFileExists = false
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                LastPath = Directory.GetParent(dialog.FileName).ToString();
                return dialog.FileName;
            } else {
                return null;
            }
        }

        //static string? lastFilePath = null;
        public static string? OpenFileDialogToGetPath(bool Load = false) {
            using CommonOpenFileDialog dialog = new CommonOpenFileDialog {
                InitialDirectory = LastPath ?? "C:\\Users",
                EnsureFileExists = Load
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                LastPath = Directory.GetParent(dialog.FileName).ToString();
                return dialog.FileName;
            } else {
                return null;
            }
        }

        //static string lastFilePath2;
        public static (bool success, string? name, string? content)
            OpenFileDialogToGetNameAndContent(string filter = "All files (*.*)|*.*", string? title = null) {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (title != null)
                openFileDialog.Title = title;
            openFileDialog.InitialDirectory = LastPath ?? "c:\\";
            openFileDialog.Filter = filter;
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            string filePath, fileContent;
            if (openFileDialog.ShowDialog() == true) {
                //Get the path of specified file
                filePath = openFileDialog.FileName;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream)) {
                    fileContent = reader.ReadToEnd();
                }

                LastPath = Directory.GetParent(openFileDialog.FileName).ToString();

                return (
                    true,
                    Path.GetFileNameWithoutExtension(filePath),
                    fileContent
                );
            } else {
                return (false, null, null);
            }
        }

        static string ConfirmCaption => "Confirmation";
        static string ErrorCaption => "ERROR!";

        public static bool WarnAndDecide(string msg) {
            MessageBoxResult result = MessageBox.Show(
                msg,
                ConfirmCaption,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }

        public static void Error(string msg) {
            MessageBox.Show(
                msg,
                ErrorCaption,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
