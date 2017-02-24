using PSTaskSupervisor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTaskSupervisor.Services
{
    public class LogMessageService
    {
        public event Action<LogMessage> OnMessagePushed = delegate { };

        public void PushMessage(string message, LogMessageLevel level)
        {
            OnMessagePushed(new LogMessage
            {
                Text = message,
                Level = level
            });
        }
    }
}
