using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.IO;
using PSTaskSupervisor.Model;
using System.Threading;

namespace PSTaskSupervisor.Services
{
    public class ScriptRunnerService
    {
        private static Task workingTask = null;
        private CancellationTokenSource waitStopTokenSource;

        private readonly LogMessageService logMessageService;
        private readonly ScriptLocatorService scriptLocatorService;

        public ScriptRunnerService(LogMessageService logMessageService,
                                   ScriptLocatorService scriptLocatorService)
        {
            this.logMessageService = logMessageService;
            this.scriptLocatorService = scriptLocatorService;
        }

        public void ForceRun()
        {
            scriptLocatorService.ClearLastRun();

            if(waitStopTokenSource != null)
                waitStopTokenSource.Cancel();
        }

        public void TryStart()
        {
            if (workingTask != null)
            {
                return;
            }

            workingTask = Task.Run(async () =>
            {
                while (true)
                {
                    foreach (var script in scriptLocatorService.KnownScripts.Where(s => s.LastRun == null ||
                                                                                        s.LastRun.Value + s.Interval <= DateTime.Now))
                    {
                        logMessageService.PushMessage($"Starting script '{script.Name}'", Model.LogMessageLevel.Info);

                        try
                        {
                            using (var psInstance = PowerShell.Create())
                            {
                                psInstance.Streams.Error.DataAdded += (s, e) =>
                                {
                                    logMessageService.PushMessage(psInstance.Streams.Error.Last().ToString(), LogMessageLevel.Error);
                                };

                                psInstance.Streams.Warning.DataAdded += (s, e) =>
                                {
                                    logMessageService.PushMessage(psInstance.Streams.Warning.Last().ToString(), LogMessageLevel.Warning);
                                };

                                psInstance.Streams.Information.DataAdded += (s, e) =>
                                {
                                    logMessageService.PushMessage(psInstance.Streams.Information.Last().ToString(), LogMessageLevel.Info);
                                };

                                var scriptContent = File.ReadAllText(script.Path);
                                psInstance.AddScript(scriptContent);
                                psInstance.Invoke();
                            }

                            logMessageService.PushMessage($"Script '{script.Name}' completed", Model.LogMessageLevel.Success);
                        }
                        catch (Exception e)
                        {
                            logMessageService.PushMessage($"Error running powershell script! {e.Message}", Model.LogMessageLevel.Error);
                        }
                        finally
                        {
                            script.LastRun = DateTime.Now;
                            script.RaisePropertyChanged("LastRun");
                        }
                    }

                    try { 
                        waitStopTokenSource = new CancellationTokenSource();
                        await Task.Delay(TimeSpan.FromMinutes(1), waitStopTokenSource.Token);
                    }catch(TaskCanceledException) { }
                }

            });
        }
    }
}
