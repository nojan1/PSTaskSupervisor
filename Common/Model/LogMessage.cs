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
        public DateTime Timestamp { get; set; }
        public string Text { get; set; }
        public string TextWithTimestamp { get => $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {Text}"; }
        public LogMessageLevel Level { get; set; }
    }
}
