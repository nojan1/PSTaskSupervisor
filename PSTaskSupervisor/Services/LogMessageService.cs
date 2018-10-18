using PSTaskSupervisor.Common.Services;
using PSTaskSupervisor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTaskSupervisor.Services
{
    public class LogMessageService : IMessageService
    {
        public event Action<LogMessage> OnMessagePushed = delegate { };

        private readonly List<LogMessage> backlog = new List<LogMessage>();

        private readonly AlertService alertService;
        public LogMessageService(AlertService alertService)
        {
            this.alertService = alertService;
        }

        public void PushMessage(string message, LogMessageLevel level)
        {
            if(level == LogMessageLevel.Error)
            {
                alertService.Alert();
            }

            PushMessage(new LogMessage
            {
                Timestamp = DateTime.Now,
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
