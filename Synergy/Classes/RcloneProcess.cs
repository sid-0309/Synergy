using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synergy.Classes
{
    public class RcloneProcess
    {
        public static Process InitializeProcess(string args)
        {

            Process Process = new();
            ProcessStartInfo StartInfo = new()
            {
                FileName = "cmd.exe",
                Arguments = $"/c rclone {args}",
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process.StartInfo = StartInfo;

            return Process;
        }
    }
}
