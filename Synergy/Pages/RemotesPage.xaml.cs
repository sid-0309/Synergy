using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using Synergy.Classes;
using Synergy.ViewModels;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Synergy.Windows;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Synergy.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RemotesPage : Page
    {
        private Process process;
        public RemotesPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.ViewModel = new RemoteViewModel();
        }

        public RemoteViewModel ViewModel
        {
            get; set;
        }

        private void remotesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            RemoteInfo EventArgs = new((string)e.ClickedItem+":", true);
            this.Frame.Navigate(typeof(RemoteContent),EventArgs);
        }

        private void Upload_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = e.OriginalSource as Button;
            string remoteRequested = clickedButton.DataContext as string;
            Window uploadWindow = new PopUpWindow(0,destination: remoteRequested + ":");

            uploadWindow.Activate();
        }
        private void Download_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = e.OriginalSource as Button;
            string remoteRequested = clickedButton.DataContext as string;
            Window downloadWindow = new PopUpWindow(1, source:remoteRequested+":", type: "directory", name:remoteRequested);

            downloadWindow.Activate();
        }

        private void Sync_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = e.OriginalSource as Button;
            string remoteRequested = clickedButton.DataContext as string;
            Window syncWindow = new PopUpWindow(2, source: remoteRequested + ":", type: "directory", name: remoteRequested);

            syncWindow.Activate();
        }

        private void Serve_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = e.OriginalSource as Button;
            string remoteRequested = clickedButton.DataContext as string;
            Window serveWindow = new PopUpWindow(3, source: remoteRequested + ":", type: "directory", name: remoteRequested);

            serveWindow.Activate();
        }

        private async void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = e.OriginalSource as Button;
            string remoteRequested = clickedButton.DataContext as string;

            ContentDialog confirmDelete = new()
            {
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Delete",
                Content = "Are you sure you want to delete this remote?",
                Height = 70,
                XamlRoot = this.XamlRoot
            };

            ContentDialogResult result = await confirmDelete.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                process = RcloneProcess.InitializeProcess($"config delete {remoteRequested}");
                if (process.Start())
                {
                    Singleton.Instance.AddTask = process;
                }
                await process.WaitForExitAsync();

            }
            else
            {
                return;
            }
            
        }
    }
}