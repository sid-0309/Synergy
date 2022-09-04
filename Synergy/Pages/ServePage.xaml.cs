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
using System.Diagnostics;


namespace Synergy.Pages
{
    public sealed partial class ServePage : Page
    {
        private Arguments receivedArguments;
        private IntPtr windowID;
        private Process process;
        private string selectedProtocol;
        private string ip = "127.0.0.1";
        private string port = "8080";
        private string username;
        private string password;
        private List<string> protocols = new List<string> { "ftp", "http", "webdav", "dlna", "sftp", "docker", "restic" };
        public ServePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            receivedArguments = e.Parameter as Arguments;
            windowID = (IntPtr)receivedArguments.Args;
        }

        private void protocolDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox protocolComboBox = sender as ComboBox;
            selectedProtocol = protocols[protocolComboBox.SelectedIndex];
            if (selectedProtocol == "REST API") { selectedProtocol = "restic"; }
            selectedProtocol.ToLower();
            if(selectedProtocol == "ftp" || selectedProtocol == "sftp" || selectedProtocol == "http" || selectedProtocol == "webdav")
            {
                uname.IsEnabled = true;
                passwd.IsEnabled = true;
            }
            else
            {
                uname.IsEnabled = false;
                passwd.IsEnabled = false;
            }
        }

        private void ipBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ip = ipBox.Text;
        }

        private void portBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            port = portBox.Text;
        }

        private void uname_TextChanged(object sender, TextChangedEventArgs e)
        {
            username = uname.Text;
        }
        async private void Start_Serve(object sender, RoutedEventArgs e)
        {
            try
            {
                if(process.HasExited == false) 
                { 
                    process.Kill();
                    start_stop.Content = "Start";
                    return;
                }
            }
            catch (NullReferenceException) 
            {
                if (username != null && password != null)
                {
                    process = RcloneProcess.InitializeProcess($"serve {selectedProtocol} {receivedArguments.Source} --addr {ip}:{port} --user {username} --pass {password}");

                }
                else
                {
                    process = RcloneProcess.InitializeProcess($"serve {selectedProtocol} \"{receivedArguments.Source}\" --addr {ip}:{port}");
                }

                if (process.Start())
                {
                    Singleton.Instance.AddTask = process;
                    start_stop.Content = "Stop";
                }
                process.Exited += Process_Exited;
                await process.WaitForExitAsync();
            }


        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Singleton.Instance.RemoveTask = process;
        }

        private void passwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            password = passwd.Password;
        }
    }
}
