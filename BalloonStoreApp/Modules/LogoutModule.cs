using BalloonStoreApp.Views;
using Prism.Modularity;
using Prism.Regions;
using BalloonStoreApp.ViewModels;
using System.Windows.Controls;
using BalloonStoreApp.Generic;

namespace BalloonStoreApp.Modules
{
    class LogoutModule : BaseModule {
        public LogoutModule(IRegionManager regionManager) : base(regionManager) {}

        override public void Initialize() {
           AddView<LogoutView, LogoutViewModel>("LogoutRegion");
        }
    }
}
