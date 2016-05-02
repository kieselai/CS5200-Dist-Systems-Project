using BalloonStoreApp.Views;
using Prism.Modularity;
using Prism.Regions;
using BalloonStoreApp.ViewModels;
using System.Windows.Controls;
using BalloonStoreApp.Generic;
using BalloonStore;

namespace BalloonStoreApp.Modules
{
    class MainModule : BaseModule {
        public MainModule(IRegionManager regionManager) : base(regionManager) {}

        override public void Initialize() {
           AddView<LoginView, LoginViewModel>("MainRegion");
           AddView<StatusView, StatusViewModel>("MainRegion");
        }
    }
}
