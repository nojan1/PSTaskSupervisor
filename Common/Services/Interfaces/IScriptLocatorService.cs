using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSTaskSupervisor.Model;

namespace PSTaskSupervisor.Common.Services
{
    public interface IScriptLocatorService
    {
        ICollection<PowershellScript> KnownScripts { get; }

        event Action<ICollection<PowershellScript>> OnScriptsUpdated;

        void ClearLastRun();
        void ClearLastRun(PowershellScript script);
        Task ScanScriptFolder();
    }
}