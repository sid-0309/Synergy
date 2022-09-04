using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Synergy.Classes;

namespace Synergy.ViewModels
{
    public class LoadRemote
    {
        private Dictionary<string, string> Remote_items = new();
        // private List<string> remoteData = new();
        public LoadRemote(string remoteName, bool isRoot)
        {
            // List<string> Remotedata = new();
            Process process = RcloneProcess.InitializeProcess("lsf " + remoteName);
            process.Start();
            process.WaitForExitAsync();
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line.Contains('/'))
                {
                    // Remotedata.Add(line.Substring(0, line.Length - 1));
                    Remote_items.Add(line.Substring(0, line.Length - 1), "directory");
                }
                else
                {
                    // Remotedata.Add(line);
                    Remote_items.Add(line, "file");
                }
            }
            // this.remoteData = Remotedata;
        }

        public Dictionary<string, string> RemoteContentViewModel { get { return Remote_items; } }

        // public List<string> RemoteData { get { return remoteData; } }

        // public Dictionary<string, string> remote_items { get { return Remote_items; } }


    }

    
}
