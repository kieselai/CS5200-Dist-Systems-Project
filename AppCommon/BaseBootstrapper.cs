using Prism.Unity;
using Prism.Modularity;
using AppCommon.Modules;
using System.Windows;
using AppCommon.ViewModels;

namespace AppCommon
{
    public abstract class BaseBootstrapper : UnityBootstrapper {
        protected override void ConfigureModuleCatalog(){
            RegisterModule<LogModule>();
            RegisterModule<MessageModule>();
            RegisterModule<CommonInfoModule>();
            RegisterModule<LogoutModule>();
        }

        protected void RegisterModule<T>() where T : IModule {
            ((ModuleCatalog)ModuleCatalog).AddModule( typeof(T) );
        }

        protected override void InitializeShell() {
            Application.Current.MainWindow.Show();
        }

        public static void BootStrapAndConfig<bootstrapper, mainWindow, mainWindowViewModel>() 
                where bootstrapper : BaseBootstrapper, new() where mainWindow : Window, new() where mainWindowViewModel : AbstractMainWindowViewModel, new() {
            bootstrapper boot = new bootstrapper();
            boot.Run();
            Application.Current.MainWindow.DataContext = new mainWindowViewModel();
            (Application.Current.MainWindow.DataContext as mainWindowViewModel).initialize();
        }
    }
}