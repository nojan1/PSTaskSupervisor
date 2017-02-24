using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTaskSupervisor.Services
{
    public class ScriptRunnerService
    {
        private static Task workingTask = null;

        private readonly LogMessageService logMessageService;
        private readonly ScriptLocatorService scriptLocatorService;

        public ScriptRunnerService(LogMessageService logMessageService,
                                   ScriptLocatorService scriptLocatorService)
        {
            this.logMessageService = logMessageService;
            this.scriptLocatorService = scriptLocatorService;
        }

        public void Start()
        {
            if(workingTask != null)
            {
                throw new Exception("Script running process allready springing");
            }

            workingTask = Task.Run(async () =>
            {
                while (true)
                {
                    foreach(var script in scriptLocatorService.KnownScripts.Where(s => s.LastRun == null || s.LastRun.Value + s.Interval <= DateTime.Now))
                    {
                        script.LastRun = DateTime.Now;

                        logMessageService.PushMessage($"Starting script {script.Name}", Model.LogMessageLevel.Info);

                    }

                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
                
            });
        }
    }
}
