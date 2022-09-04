using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Synergy.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using static System.Net.WebRequestMethods;

namespace Synergy.Pages
{

    public sealed partial class DownloadPage : Page
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
        public DownloadPage()
        {
            this.InitializeComponent();
        }

        async private void Open_Folder(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, windowID);

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder == null) return;


            int index = folder.Path.IndexOf($"\\{folder.Name}\\");
            receivedArguments.Destination = (index < 0)
                ? folder.Path
                : folder.Path.Remove(index, $"\\{folder.Name}\\".Length);


            process = RcloneProcess.InitializeProcess($"lsf \"{folder.Path}\"");
            process.Start();
            await process.WaitForExitAsync();
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();

                if (line.Contains('/') && line.Contains(receivedArguments.Name) && receivedArguments.Type == "directory")
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
                    if (result == ContentDialogResult.Primary)
                    {
                        folderExists = true;
                        break;
                    }
                    else { return; }
                }
                else if (line.Contains(receivedArguments.Name) && receivedArguments.Type == "file")
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
            if (!folderExists && receivedArguments.Type=="directory")
            {
                process = RcloneProcess.InitializeProcess($"mkdir \"{folder.Path}/{receivedArguments.Name}\"");
                process.Start();
                await process.WaitForExitAsync();
            }
            openedFileorFolder.Text = folder.Path;
            openedFileorFolder.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.WhiteSmoke);

            if (receivedArguments.Type == "directory") { process = RcloneProcess.InitializeProcess($"copy \"{receivedArguments.Source}\" \"{folder.Path}/{receivedArguments.Name}\""); }
            else
            {
                process = RcloneProcess.InitializeProcess($"copy \"{receivedArguments.Source}\" \"{folder.Path}\"");
            }            
            downloadBtn.IsEnabled = true;
        }

        async private void Download(object sender, RoutedEventArgs e)
        {
            if (process.Start())
            {
                Singleton.Instance.AddTask = process;
            }
            await process.WaitForExitAsync();
        }
    }
}
