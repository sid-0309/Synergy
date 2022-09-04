using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synergy.Classes
{
    public sealed class Singleton
    {
        private static readonly Lazy<Singleton> instance = new Lazy<Singleton>(() => new Singleton());
        public static Singleton Instance { get { return instance.Value; } }

        private List<System.Diagnostics.Process> CurrentTasks = new();
        private List<System.Diagnostics.Process> SavedTasks = new();

        public List<System.Diagnostics.Process> GetTasks
        {
            get { return CurrentTasks; }
        }
        public System.Diagnostics.Process AddTask
        {
            set { CurrentTasks.Add(value); }
        }

        public System.Diagnostics.Process RemoveTask
        {
            set { CurrentTasks.Remove(value); }
        }
        private Singleton()
        {

        }
    }
}
