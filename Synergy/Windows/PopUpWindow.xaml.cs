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

using Synergy.Pages;
using Microsoft.UI;
using Synergy.Classes;
using Microsoft.UI.Windowing;
using Windows.Graphics;


namespace Synergy.Windows
{

    public sealed partial class PopUpWindow : Window
    {
        private RectInt32 windowSize;
        public PopUpWindow(int page, string type = null, string source = null,string name = null, string destination = null, string command = null, object args = null)
        {
            this.InitializeComponent();
            IntPtr windowID = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowID);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            windowSize.Height = 300;
            windowSize.Width = 735;
            windowSize.X = 593;
            windowSize.Y = 379;
            appWindow.MoveAndResize(windowSize);

            Arguments arguments = new()
            {
                Name = name,
                Destination = destination,
                Command = command,
                Args = windowID,
                Type = type,
                Source = source,
            };

            OverlappedPresenter overlappedPresenter = appWindow.Presenter as OverlappedPresenter;
            overlappedPresenter.IsResizable = false;

            switch (page)
            {
                case 0:
                    popUpFrame.Navigate(typeof(UploadPage), arguments);
                    break;
                case 1:
                    popUpFrame.Navigate(typeof(DownloadPage), arguments);
                    break;
                case 2:
                    popUpFrame.Navigate(typeof(SyncPage), arguments);
                    break;
                case 3:
                    popUpFrame.Navigate(typeof(ServePage), arguments);
                    break;
            }
            
        }
    }
}
