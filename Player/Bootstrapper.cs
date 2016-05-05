using Microsoft.Practices.Unity;
using Player.Views;
using System.Windows;
using Player.Modules;
using AppCommon;

namespace Player
{
    public class Bootstrapper : BaseBootstrapper {
        protected override DependencyObject CreateShell() {
            return Container.Resolve<MainWindow>();
        }
        protected override void ConfigureModuleCatalog() {
            RegisterModule<MainModule>();
            base.ConfigureModuleCatalog();
            RegisterModule<OwnInfoModule>();
        }
    }
}
