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

using Synergy.Classes;
using Microsoft.UI;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Synergy.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SyncPage : Page
    {
        private Arguments receivedArguments;
        private IntPtr windowID;
        private Process process;
        private bool folderExists;
        private StorageFolder folder;
        public SyncPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            receivedArguments = e.Parameter as Arguments;
            windowID = (IntPtr)receivedArguments.Args;
        }

        async private void Open_Folder(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, windowID);

            folder = await folderPicker.PickSingleFolderAsync();
            if (folder == null) return;

            int index = folder.Path.IndexOf($"\\{folder.Name}\\");
            receivedArguments.Destination = (index < 0)
                ? folder.Path
                : folder.Path.Remove(index, $"\\{folder.Name}\\".Length);

            receivedArguments.Destination = folder.Path;


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
            }
            if (!folderExists && receivedArguments.Type == "directory")
            {
                process = RcloneProcess.InitializeProcess($"mkdir \"{folder.Path}/{receivedArguments.Name}\"");
                process.Start();
                await process.WaitForExitAsync();
            }

            openedFileorFolder.Text = folder.Path;
            openedFileorFolder.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.WhiteSmoke);

            process = RcloneProcess.InitializeProcess($"sync \"{receivedArguments.Source}\" \"{receivedArguments.Destination}/{receivedArguments.Name}\"");

        }

        async private void Sync(object sender, RoutedEventArgs e)
        {
            if (process.Start())
            {
                Singleton.Instance.AddTask = process;
            }
            process.Exited += Process_Exited;
            await process.WaitForExitAsync();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Singleton.Instance.RemoveTask = process;
        }

        private void Swap_button_click(object sender, RoutedEventArgs e)
        {
            string temp = receivedArguments.Source;
            receivedArguments.Source = folder.Path;
            receivedArguments.Destination = temp;
            remotePath.Text = receivedArguments.Source;
            openedFileorFolder.Text = receivedArguments.Destination;
        }
    }
}
