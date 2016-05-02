using Microsoft.Practices.Unity;
using BalloonStore.Views;
using System.Windows;
using BalloonStore.Modules;
using AppCommon;

namespace BalloonStore
{
    public class Bootstrapper : BaseBootstrapper {
        protected override DependencyObject CreateShell() {
            return Container.Resolve<MainWindow>();
        }
        protected override void ConfigureModuleCatalog() {
            base.ConfigureModuleCatalog();
            RegisterModule<OwnInfoModule>();
            RegisterModule<LogoutModule>();
        }
    }
}
