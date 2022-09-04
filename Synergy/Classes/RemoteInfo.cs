using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synergy.Classes
{
    internal class RemoteInfo
    {
        private string name;
        private bool isRoot;
        public RemoteInfo(string name, bool isRoot)
        {
            this.name = name;
            this.isRoot = isRoot;
        }
        public string Name { get { return name; } set { name = value; } }
        public bool IsRoot { get { return isRoot; } set { isRoot = value; } }
    }
}
