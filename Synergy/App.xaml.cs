using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Synergy.Windows;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using Windows.UI.Core.Preview;

using Synergy.Classes;


namespace Synergy
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            var RuntimeData = Singleton.Instance;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            InitializeTrayIcon();
            m_window = new MainWindow();
            m_window.Activate();
        }
        

        private void InitializeTrayIcon()
        {
            var showHideWindowCommand = (XamlUICommand)Resources["ShowHideWindowCommand"];
            showHideWindowCommand.ExecuteRequested += ShowHideWindowCommand_ExecuteRequested;

            var exitApplicationCommand = (XamlUICommand)Resources["ExitApplicationCommand"];
            exitApplicationCommand.ExecuteRequested += ExitApplicationCommand_ExecuteRequested;

            tray_icon = (TaskbarIcon)Resources["trayIcon"];
            tray_icon.ForceCreate();
        }

        private void ShowHideWindowCommand_ExecuteRequested(object _, ExecuteRequestedEventArgs args)
        {
            if (m_window == null)
            {
                m_window = new Window();
                m_window.Show();
                return;
            }

            if (m_window.Visible)
            {
                m_window.Hide();
            }
            else
            {
                m_window.Show();
            }
        }

        private void ExitApplicationCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
        {
            tray_icon?.Dispose();
            m_window?.Close();
        }

        private Window m_window;
        public TaskbarIcon tray_icon = new();

    }
}
