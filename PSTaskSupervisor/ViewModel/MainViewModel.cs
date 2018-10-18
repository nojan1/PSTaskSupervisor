using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PSTaskSupervisor.Common.Services;
using PSTaskSupervisor.Model;
using PSTaskSupervisor.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

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
                    _alertService.ClearAlert();
                });
            }
        }

        public ICommand LoadScripts
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await _scriptLocatorService.ScanScriptFolder();
                    _scriptRunnerService.TryStart();
                });
            }
        }

        public ICommand ForceScriptRun
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _scriptRunnerService.ForceRun();
                });
            }
        }

        public ICommand ForceSingleScriptRun
        {
            get
            {
                return new RelayCommand<PowershellScript>((script) =>
                {
                    _scriptRunnerService.ForceRun(script);
                });
            }
        }

        private readonly IScriptLocatorService _scriptLocatorService;
        private readonly IScriptRunnerService _scriptRunnerService;
        private readonly AlertService _alertService;

        public MainViewModel(IScriptLocatorService scriptLocatorService,
                             IScriptRunnerService scriptRunnerService,
                             AlertService alertService)
        {
            _alertService = alertService;
            _scriptLocatorService = scriptLocatorService;
            _scriptRunnerService = scriptRunnerService;

            _scriptLocatorService.OnScriptsUpdated += x =>
            {
                Scripts = new ObservableCollection<PowershellScript>(x);
            };

            _scriptRunnerService.ScriptRunComplete += x =>
            {
                MainWindow.RootDispatcher.Invoke(() =>
                {
                    //HACK! This forces update of the LastRun property. Correct way would be to implement INotifyPropertyChanged in derived class.
                    int index = Scripts.IndexOf(x);
                    if (index != -1)
                    {
                        Scripts.RemoveAt(index);
                        Scripts.Insert(index, x);
                    }
                });
            };
        }
    }
}