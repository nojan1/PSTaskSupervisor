using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PSTaskSupervisor.Model;
using PSTaskSupervisor.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSTaskSupervisor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<PowershellScript> scripts;
        public ObservableCollection<PowershellScript> Scripts { get { return scripts; } set { Set(ref scripts, value); } }

        public ICommand LoadScripts
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await scriptLocatorService.ScanScriptFolder();
                    scriptRunnerService.TryStart();
                });
            }
        }

        private readonly ScriptLocatorService scriptLocatorService;
        private readonly ScriptRunnerService scriptRunnerService;

        public MainViewModel(ScriptLocatorService scriptLocatorService,
                             ScriptRunnerService scriptRunnerService)
        {
            this.scriptLocatorService = scriptLocatorService;
            this.scriptRunnerService = scriptRunnerService;

            scriptLocatorService.OnScriptsUpdated += x =>
            {
                Scripts = new ObservableCollection<PowershellScript>(x);
            };
        }
    }
}