using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synergy.Classes;

namespace Synergy.ViewModels;

public class LoadConfig : List<string>
{
    private string defaultPath;
    private List<string> remotes;

    public LoadConfig()
    {
        List<string> remotes = new List<string>();
        string defaultPath = (string)Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\rclone\\rclone.conf";

        string[] lines = System.IO.File.ReadAllLines(defaultPath);
        foreach (string line in lines)
        {
            if (line.Contains('[') && line.Contains(']'))
            {
                string temp = line.Substring(line.IndexOf("[")+1, line.IndexOf("]")-1);
                remotes.Add(temp);
            }
        }

        this.defaultPath = defaultPath;
        this.remotes = remotes;
    }
    public List<string> Remotes
    {
        get { return remotes; }
    }


}
public class RemoteViewModel
{
    private LoadConfig remotes_list = new();
    public LoadConfig Remtoes_list { get { return remotes_list;}}  
}