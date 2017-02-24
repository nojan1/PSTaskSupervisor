using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTaskSupervisor.Model
{
    public class PowershellScript : ObservableObject
    {
        public string Name { get; set; }
        public TimeSpan Interval { get; set; }
        public string Path { get; set; }
        public DateTime? LastRun { get; set; }
    }
}