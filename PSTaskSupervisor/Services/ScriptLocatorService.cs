using PSTaskSupervisor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTaskSupervisor.Services
{
    public class ScriptLocatorService
    {
        public ICollection<PowershellScript> KnownScripts { get; private set; }

        public event Action<ICollection<PowershellScript>> OnScriptsUpdated = delegate { };

        private readonly LogMessageService logMessageService;
        public ScriptLocatorService(LogMessageService logMessageService)
        {
            this.logMessageService = logMessageService;
        }

        public void ClearLastRun()
        {
            foreach(var script in KnownScripts)
            {
                script.LastRun = null;
                script.RaisePropertyChanged("LastRun");
            }
        }

        public void ClearLastRun(PowershellScript script)
        {
            script.LastRun = null;
            script.RaisePropertyChanged("LastRun");
        }

        public async Task ScanScriptFolder()
        {
            var scripts = new List<PowershellScript>();

            await Task.Run(() =>
            {
                foreach (var scriptFilePath in Directory.EnumerateFiles("scripts", "*.ps1"))
                {
                    var infoLines = File.ReadAllLines(scriptFilePath).TakeWhile(l => l.StartsWith("#"));

                    var interval = ParseInfoLine<TimeSpan?>(infoLines, "interval", v =>
                    {
                        TimeSpan value;
                        if (!string.IsNullOrWhiteSpace(v) && TimeSpan.TryParse(v, out value))
                        {
                            return value;
                        }
                        else
                        {
                            return null;
                        }
                    });

                    scripts.Add(new PowershellScript
                    {
                        Path = scriptFilePath,
                        Name = new FileInfo(scriptFilePath).Name,
                        Interval = interval.HasValue ? interval.Value : TimeSpan.FromMinutes(30),
                        LastRun = KnownScripts?.FirstOrDefault(s => s.Path == scriptFilePath)?.LastRun
                    });
                }
            });

            logMessageService.PushMessage($"Loaded {scripts.Count} scripts", LogMessageLevel.Info);

            KnownScripts = scripts;
            OnScriptsUpdated(scripts);
        }

        private T ParseInfoLine<T>(IEnumerable<string> infoLines, string valueName, Func<string, T> valueExtractor)
        {
            var line = infoLines.FirstOrDefault(l => l.ToLower().StartsWith($"#{valueName.ToLower()}:"));
            if (line == null)
            {
                return default(T);
            }

            var parts = line.Split(':').Select(t => t.Trim());
            var secondHalf = string.Join(":", parts.Skip(1));
            return valueExtractor(secondHalf);
        }
    }
}
