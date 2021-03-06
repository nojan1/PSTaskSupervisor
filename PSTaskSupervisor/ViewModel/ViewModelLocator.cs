/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:PSTaskSupervisor"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using PSTaskSupervisor.Common.Services;
using PSTaskSupervisor.Services;

namespace PSTaskSupervisor.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IMessageService, LogMessageService>();
            SimpleIoc.Default.Register<IScriptLocatorService, ScriptLocatorService>();
            SimpleIoc.Default.Register<IScriptRunnerService, ScriptRunnerService>();
            SimpleIoc.Default.Register<AlertService>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LogWindowViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public LogWindowViewModel LogWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LogWindowViewModel>();
            }
        }

        public AlertService AlertService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AlertService>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}