using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PSTaskSupervisor.Model;
using PSTaskSupervisor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSTaskSupervisor.ViewModel
{
    public class LogWindowViewModel : ViewModelBase
    {
        const int MAX_MESSAGES = 100;

        public ObservableCollection<LogMessage> LogMessages { get; private set; } = new ObservableCollection<LogMessage>();

        public ICommand ClearLog
        {
            get
            {
                return new RelayCommand(() =>
                {
                    LogMessages.Clear();
                });
            }
        }

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
