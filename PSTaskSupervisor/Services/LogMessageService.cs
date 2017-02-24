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

        private List<LogMessage> backlog = new List<LogMessage>();

        public void PushMessage(string message, LogMessageLevel level, bool prependTimestamp = true)
        {
            if (prependTimestamp)
            {
                message = $"{DateTime.Now.ToString("yy-MM-dd HH:mm:ss")} - {message}";
            }

            PushMessage(new LogMessage
            {
                Text = message,
                Level = level
            });
        }

        public void PushMessage(LogMessage message)
        {
            backlog.Add(message);

            if (MainWindow.RootDispatcher != null)
            {
                foreach(var backlogMessage in backlog)
                {
                    MainWindow.RootDispatcher.Invoke(() =>
                    {
                        OnMessagePushed(backlogMessage);
                    });
                }

                backlog.Clear();
            }
        }
    }
}
