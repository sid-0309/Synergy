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
using Windows.UI.Popups;
using Windows.UI.Core.Preview;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Synergy
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            rootFrame.Navigate(typeof(RemotesPage));
        }

        private void nav_bar_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            switch (sender.MenuItems.IndexOf(args.InvokedItemContainer))
            {
                case 1:
                    rootFrame.Navigate(typeof(RemotesPage));
                    break;

                case 2:
                    rootFrame.Navigate(typeof(TasksPage));
                    break;
                case 3:
                    rootFrame.Navigate(typeof(Scheduler));
                    break;

                default:
                    rootFrame.Navigate(typeof(RemotesPage));
                    break;
            }
        }

        private void nav_bar_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
            else
            {
                rootFrame.Navigate(typeof(RemotesPage));
            }
        }
    }
}
