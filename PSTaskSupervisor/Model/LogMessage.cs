using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTaskSupervisor.Model
{
    public enum LogMessageLevel
    {
        Success,
        Info,
        Warning,
        Error
    }

    public class LogMessage
    {
        public string Text { get; set; }
        public LogMessageLevel Level { get; set; }
    }
}
