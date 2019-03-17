using Microsoft.Win32;
using System.IO;

using System.Windows;

using Microsoft.WindowsAPICodePack.Dialogs;

#nullable enable

namespace NnManagerGUI.ViewModel
{
    static class UtilGUI
    {
        static string lastFolderPath;
        public static string? OpenFileDialogToGetFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog {
                InitialDirectory = lastFolderPath ?? "C:\\Users",
                IsFolderPicker = true,
                EnsureFileExists = false
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                lastFolderPath = Directory.GetParent(dialog.FileName).ToString();
                return dialog.FileName;
            } else {
                return null;
            }
        }

        static string lastFilePath;
        public static string? OpenFileDialogToGetPath(bool Load = false)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog {
                InitialDirectory = lastFilePath ?? "C:\\Users",
                EnsureFileExists = Load
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                lastFilePath = Directory.GetParent(dialog.FileName).ToString();
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        static string lastFilePath2;
        public static (bool success, string? name, string? content) 
            OpenFileDialogToGetNameAndContent(string filter = "All files (*.*)|*.*", string? title = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (title != null)
                openFileDialog.Title = title;
            openFileDialog.InitialDirectory = lastFilePath2 ?? "c:\\";
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

                lastFilePath2 = Directory.GetParent(openFileDialog.FileName).ToString();

                return (
                    true,
                    Path.GetFileNameWithoutExtension(filePath),
                    fileContent
                );
            } else {
                return (false, null, null);
            }
        }

        public static bool WarnAndDecide(string msg)
        {
            MessageBoxResult result = MessageBox.Show(
                msg,
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }

        public static void Error(string msg)
        {
            MessageBox.Show(
                msg,
                "ERROR!",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
