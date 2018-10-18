using PSTaskSupervisor.Model;
using System;

namespace PSTaskSupervisor.Common.Services
{
    public interface IScriptRunnerService
    {
        event Action<PowershellScript> ScriptRunComplete;
        void ForceRun();
        void ForceRun(PowershellScript script);
        void TryStart();
    }
}