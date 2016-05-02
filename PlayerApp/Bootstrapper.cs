using Microsoft.Practices.Unity;
using PlayerApp.Views;
using System.Windows;
using PlayerApp.Modules;
using AppCommon;


namespace PlayerApp
{
    public class Bootstrapper : BaseBootstrapper {
        protected override DependencyObject CreateShell() {
            return Container.Resolve<MainWindow>();
        }
        protected override void ConfigureModuleCatalog() {
            RegisterModule<MainModule>();
            base.ConfigureModuleCatalog();
            RegisterModule<OwnInfoModule>();
            RegisterModule<LogoutModule>();
        }
    }
}
