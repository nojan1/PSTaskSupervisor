using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PSTaskSupervisor.Common.Services;
using PSTaskSupervisor.Model;
using PSTaskSupervisor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace PSTaskSupervisor.ViewModel
{
    public class LogWindowViewModel : ViewModelBase
    {
        const int MAX_MESSAGES = 100;

        public event EventHandler<LogMessage> OnLogMessageSelected = delegate {};
        public ObservableCollection<LogMessage> LogMessages { get; private set; } = new ObservableCollection<LogMessage>();

        public ICommand ClearLog
        {
            get => new RelayCommand(() =>
                {
                    LogMessages.Clear();
                });
        }

        public ICommand SelectMessage
        {
            get => new RelayCommand<SelectionChangedEventArgs>((eventargs) =>
            {
                if (eventargs.AddedItems.Count == 0)
                    return;

                var message = (LogMessage)eventargs.AddedItems[0];

                OnLogMessageSelected(this, message);
                (new LogMessageDetailWindow() { DataContext = message }).Show();
            });
        }

        public LogWindowViewModel(IMessageService logMessageService)
        {
            logMessageService.OnMessagePushed += message =>
            {
                LogMessages.Insert(0, message);

                if (LogMessages.Count > MAX_MESSAGES)
                {
                    LogMessages.RemoveAt(MAX_MESSAGES - 1);
                }
            };
        }
    }
}
