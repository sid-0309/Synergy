using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Synergy.Windows;
using Windows.Storage;
using System.Diagnostics;
using Synergy.Classes;
using static System.Net.WebRequestMethods;
using Page = Microsoft.UI.Xaml.Controls.Page;



namespace Synergy.Pages
{
    public sealed partial class UploadPage : Page
    {
        private Process process;
        private IntPtr windowID;
        private Arguments receivedArguments;
        public bool folderExists = false;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            receivedArguments = e.Parameter as Arguments;
            windowID = (IntPtr)receivedArguments.Args;
        }
        public UploadPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        async private void Open_File(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileOpenPicker();


            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, windowID);

            // Use file picker like normal!
            filePicker.FileTypeFilter.Add("*");
            StorageFile file = await filePicker.PickSingleFileAsync();
            if(file == null) return;
            int index = file.Path.IndexOf($"\\{file.Name}");
            receivedArguments.Source = (index < 0)
                ? file.Path
                : file.Path.Remove(index, $"\\{file.Name}".Length);

            // check if the file exists at the destination.
            process = RcloneProcess.InitializeProcess($"lsf \"{receivedArguments.Destination}\"");
            process.Start();
            await process.WaitForExitAsync();
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line.Contains(file.Name))
                {
                    ContentDialog contentDialog = new()
                    {
                        CloseButtonText = "Cancel",
                        PrimaryButtonText = "Proceed",
                        Content = "A file with the same name exists. Overwrite existing file?",
                        XamlRoot = this.XamlRoot
                    };
                    var result = await contentDialog.ShowAsync();
                    if (result == ContentDialogResult.Primary) { break; }
                    else { return; }
                }
            }


            process = RcloneProcess.InitializeProcess($"copy \"{file.Path}\" \"{receivedArguments.Destination}\"");
            openedFileorFolder.Text = file.Path;
            openedFileorFolder.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.WhiteSmoke);
            uploadBtn.IsEnabled = true;
        }

        async private void Open_Folder(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, windowID);

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder == null) return;

            int index = folder.Path.IndexOf($"\\{folder.Name}\\");
            receivedArguments.Source = (index < 0)
                ? folder.Path
                : folder.Path.Remove(index, $"\\{folder.Name}\\".Length);

            // Check if the folder exists at the destination.
            process = RcloneProcess.InitializeProcess($"lsf \"{receivedArguments.Destination}\"");
            process.Start();
            await process.WaitForExitAsync();
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line.Contains('/') && line.Contains(folder.Name))
                {
                    ContentDialog contentDialog = new()
                    {
                        CloseButtonText = "Cancel",
                        PrimaryButtonText = "Proceed",
                        Content = "A folder with the same name exists. Merge these folder? (Files may be overwritten)",
                        Height = 100,
                        XamlRoot = this.XamlRoot
                    };
                    ContentDialogResult result = await contentDialog.ShowAsync();
                    if(result == ContentDialogResult.Primary)
                    {
                        folderExists = true;
                        break;
                    }
                    else { return; }
                }
            }
            if (!folderExists)
            {
                process = RcloneProcess.InitializeProcess($"mkdir \"{receivedArguments.Destination}/{folder.Name}\"");
                process.Start();
                await process.WaitForExitAsync();
            }
            openedFileorFolder.Text = folder.Path;
            openedFileorFolder.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.WhiteSmoke);

            process = RcloneProcess.InitializeProcess($"copy \"{folder.Path}\" \"{receivedArguments.Destination}/{folder.Name}\"");
            uploadBtn.IsEnabled = true;

        }

        async private void Upload(object sender, RoutedEventArgs e)
        {
             process.Start();
            await process.WaitForExitAsync();
        }
    }
}