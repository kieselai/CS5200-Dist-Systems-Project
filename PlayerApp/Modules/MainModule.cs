using PlayerApp.Views;
using Prism.Modularity;
using Prism.Regions;
using PlayerApp.ViewModels;
using System.Windows.Controls;
using PlayerApp.Generic;
using Player;

namespace PlayerApp.Modules
{
    class MainModule : BaseModule {
        public MainModule(IRegionManager regionManager) : base(regionManager) {}

        override public void Initialize() {
           AddView<LoginView, LoginViewModel>("MainRegion");
           AddView<StatusView, StatusViewModel>("MainRegion");
        }
    }
}
