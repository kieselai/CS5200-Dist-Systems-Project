using Microsoft.Practices.Unity;
using Prism.Unity;
using PlayerApp.Views;
using System.Windows;
using Prism.Modularity;
using PlayerApp.Modules;
using PlayerApp.ViewModels;

namespace PlayerApp
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell() {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell() {
            Application.Current.MainWindow.Show();
        }
        protected override void ConfigureModuleCatalog() {
            ModuleCatalog catalog = (ModuleCatalog)ModuleCatalog;
            catalog.AddModule(typeof(MainModule));
            catalog.AddModule(typeof (MessageModule));
            catalog.AddModule(typeof(LogModule));
            catalog.AddModule(typeof(LogoutModule));
        }
    }
}
