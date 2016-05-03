using Player.Views;
using Prism.Modularity;
using Prism.Regions;
using Player.ViewModels;
using System.Windows.Controls;
using AppCommon.Generic;
using PlayerProcess;

namespace Player.Modules
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