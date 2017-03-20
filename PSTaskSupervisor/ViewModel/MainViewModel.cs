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

        public ICommand ClearAlarm
        {
            get
            {
                return new RelayCommand(() =>
                {
                    alertService.ClearAlert();
                });
            }
        }

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

        public ICommand ForceScriptRun
        {
            get
            {
                return new RelayCommand(() =>
                {
                    scriptRunnerService.ForceRun();
                });
            }
        }

        public ICommand ForceSingleScriptRun
        {
            get
            {
                return new RelayCommand<PowershellScript>((script) =>
                {
                    scriptRunnerService.ForceRun(script);
                });
            }
        }

        private readonly ScriptLocatorService scriptLocatorService;
        private readonly ScriptRunnerService scriptRunnerService;
        private readonly AlertService alertService;

        public MainViewModel(ScriptLocatorService scriptLocatorService,
                             ScriptRunnerService scriptRunnerService,
                             AlertService alertService)
        {
            this.alertService = alertService;
            this.scriptLocatorService = scriptLocatorService;
            this.scriptRunnerService = scriptRunnerService;

            scriptLocatorService.OnScriptsUpdated += x =>
            {
                Scripts = new ObservableCollection<PowershellScript>(x);
            };
        }
    }
}