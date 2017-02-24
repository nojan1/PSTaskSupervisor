using GalaSoft.MvvmLight;
using PSTaskSupervisor.Model;
using PSTaskSupervisor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTaskSupervisor.ViewModel
{
    public class LogWindowViewModel : ViewModelBase
    {
        const int MAX_MESSAGES = 100;

        public ObservableCollection<LogMessage> LogMessages { get; private set; } = new ObservableCollection<LogMessage>();

        public LogWindowViewModel(LogMessageService logMessageService)
        {
            logMessageService.OnMessagePushed += message =>
            {
                LogMessages.Insert(0, message);

                if(LogMessages.Count > MAX_MESSAGES)
                {
                    LogMessages.RemoveAt(MAX_MESSAGES - 1);
                }
            };
        }
    }
}
