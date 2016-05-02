using PlayerApp.Views;
using Prism.Modularity;
using Prism.Regions;
using PlayerApp.ViewModels;
using System.Windows.Controls;
using AppCommon.Generic;
using Player;

namespace PlayerApp.Modules
{
    class MainModule : BaseModule {
        public MainModule(IRegionManager regionManager) : base(regionManager) {
            RegionName = "MainRegion";
        }

        override public void Initialize() {
           AddView<LoginView, LoginViewModel>();
        }
    }
}