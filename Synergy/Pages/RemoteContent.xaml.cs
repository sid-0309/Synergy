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

using Synergy.ViewModels;
using Synergy.Classes;
using Synergy.Windows;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Synergy.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RemoteContent : Page
    {
        private Process process;

        private RemoteInfo NavigatedEventArgs;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigatedEventArgs = (RemoteInfo)e.Parameter;
            this.ViewModel = new LoadRemote(NavigatedEventArgs.Name,NavigatedEventArgs.IsRoot);
            this.remoteItems = ViewModel.RemoteContentViewModel;
        }

        public RemoteContent()
        {
            this.InitializeComponent();
        }

        public LoadRemote ViewModel { get; set; }
        private Dictionary<string, string> remoteItems;

        private void contentList_ItemClick(object sender, ItemClickEventArgs e)
        {

            RemoteInfo ClickedEventArgs = new(NavigatedEventArgs.Name + (string)e.ClickedItem,false);
            
            this.Frame.Navigate(typeof(RemoteContent),ClickedEventArgs);
        }

        private void Upload_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = e.OriginalSource as Button;
            string remoteRequested = clickedButton.DataContext as string;
            Window uploadWindow = new PopUpWindow(0, destination: NavigatedEventArgs.Name+"/" + remoteRequested + "/");

            uploadWindow.Activate();
        }

        private void Download_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = e.OriginalSource as Button;
            string remoteRequested = clickedButton.DataContext as string;
            Window downloadWindow;
            if (remoteItems[remoteRequested] == "file")
            {
                downloadWindow = new PopUpWindow(1, source: NavigatedEventArgs.Name +"/"+ remoteRequested, type: "file", name: remoteRequested);
            }
            else
            {
                downloadWindow = new PopUpWindow(1, source: NavigatedEventArgs.Name + "/" + remoteRequested + "/", type: "directory", name: remoteRequested);
            }
            

            downloadWindow.Activate();
        }

        private void Sync_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = e.OriginalSource as Button;
            string remoteRequested = clickedButton.DataContext as string;
            Window syncWindow = new PopUpWindow(2, source: NavigatedEventArgs.Name + "/" + remoteRequested + "/", type: "directory", name: remoteRequested);
            syncWindow.Activate();
        }

        private async void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = e.OriginalSource as Button;
            string remoteRequested = clickedButton.DataContext as string;

            ContentDialog confirmDelete = new()
            {
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Delete",
                Content = "Are you sure you want to delete this item?",
                Height = 70,
                XamlRoot = this.XamlRoot
            };

            ContentDialogResult result = await confirmDelete.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                process = RcloneProcess.InitializeProcess($"rmdirs {NavigatedEventArgs.Name+"/"+remoteRequested}");
                if (process.Start()) { Singleton.Instance.AddTask = process; }
                await process.WaitForExitAsync();
            }
            else
            {
                return;
            }

        }

        private void Serve_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = e.OriginalSource as Button;
            string remoteRequested = clickedButton.DataContext as string;
            Window serveWindow = new PopUpWindow(3, source: NavigatedEventArgs.Name + "/" + remoteRequested + "/", type: "directory", name: remoteRequested);
            serveWindow.Activate();
        }
    }
    
}
