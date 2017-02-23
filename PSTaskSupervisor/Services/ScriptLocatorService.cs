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
                        Interval = interval.Value
                    });
                }
            });

            KnownScripts = scripts;
            OnScriptsUpdated(scripts);
        }

        private T ParseInfoLine<T>(IEnumerable<string> infoLines, string valueName, Func<string, T> valueExtractor)
        {
            var line = infoLines.FirstOrDefault(l => l.ToLower().StartsWith($"#{l.ToLower()}:"));
            if (line == null)
            {
                return default(T);
            }

            var parts = line.Split(':');
            return valueExtractor(parts.Length == 2 ? parts[1].Trim() : "");
        }
    }
}
