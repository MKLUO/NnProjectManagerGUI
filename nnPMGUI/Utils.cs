using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace NnManagerGUI.ViewModel
{
    static class UtilGUI
    {
        public static string OpenFileDialogToGetFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            dialog.EnsureFileExists = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                return dialog.FileName;
            } else {
                return null;
            }
        }

        public static string OpenFileDialogToGetPath(bool Load = false)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.EnsureFileExists = Load;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        public static (bool, string, string) OpenFileDialogToGetNameAndContent(string filter = "All files (*.*)|*.*")
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
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
