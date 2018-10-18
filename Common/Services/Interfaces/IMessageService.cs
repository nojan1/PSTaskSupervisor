using System;
using PSTaskSupervisor.Model;

namespace PSTaskSupervisor.Common.Services
{
    public interface IMessageService
    {
        event Action<LogMessage> OnMessagePushed;

        void PushMessage(LogMessage message);
        void PushMessage(string message, LogMessageLevel level);
    }
}