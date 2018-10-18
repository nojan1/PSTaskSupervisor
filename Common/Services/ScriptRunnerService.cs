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

namespace PSTaskSupervisor.Common.Services
{
    public class ScriptRunnerService : IScriptRunnerService
    {
        public event Action<PowershellScript> ScriptRunComplete = delegate { };

        private static Task workingTask = null;
        private CancellationTokenSource waitStopTokenSource;

        private readonly IMessageService _logMessageService;
        private readonly IScriptLocatorService _scriptLocatorService;

        public ScriptRunnerService(IMessageService logMessageService,
                                   IScriptLocatorService scriptLocatorService)
        {
            _logMessageService = logMessageService;
            _scriptLocatorService = scriptLocatorService;
        }

        public void ForceRun()
        {
            ForceRun(null);
        }

        public void ForceRun(PowershellScript script)
        {
            if (script == null)
            {
                _scriptLocatorService.ClearLastRun();
            }else
            {
                _scriptLocatorService.ClearLastRun(script);
            }

            if (waitStopTokenSource != null)
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
                    foreach (var script in _scriptLocatorService.KnownScripts.Where(s => s.ShouldRun))
                    {
                        _logMessageService.PushMessage($"Starting script '{script.Name}'", Model.LogMessageLevel.Info);

                        try
                        {
                            using (var psInstance = PowerShell.Create())
                            {
                                psInstance.Streams.Error.DataAdded += (s, e) =>
                                {
                                    _logMessageService.PushMessage(psInstance.Streams.Error.Last().ToString(), LogMessageLevel.Error);
                                };

                                psInstance.Streams.Warning.DataAdded += (s, e) =>
                                {
                                    _logMessageService.PushMessage(psInstance.Streams.Warning.Last().ToString(), LogMessageLevel.Warning);
                                };

                                psInstance.Streams.Information.DataAdded += (s, e) =>
                                {
                                    _logMessageService.PushMessage(psInstance.Streams.Information.Last().ToString(), LogMessageLevel.Info);
                                };

                                var scriptContent = File.ReadAllText(script.Path);
                                psInstance.AddScript(scriptContent);
                                psInstance.Invoke();
                            }

                            _logMessageService.PushMessage($"Script '{script.Name}' completed", Model.LogMessageLevel.Success);
                        }
                        catch (Exception e)
                        {
                            _logMessageService.PushMessage($"Error running powershell script! {e.Message}", Model.LogMessageLevel.Error);
                        }
                        finally
                        {
                            script.LastRun = DateTime.Now;
                            ScriptRunComplete(script);
                        }
                    }

                    try
                    {
                        waitStopTokenSource = new CancellationTokenSource();
                        await Task.Delay(TimeSpan.FromMinutes(1), waitStopTokenSource.Token);
                    }
                    catch (TaskCanceledException) { }
                }

            });
        }
    }
}
