using Prism.Regions;
using BalloonStore.ViewModels;
using AppCommon.Generic;
using AppCommon.Views;
namespace BalloonStore.Modules
{
    class LogoutModule : BaseModule {
        public LogoutModule(IRegionManager regionManager) : base(regionManager) {
            RegionName = "LogoutRegion";
        }

        override public void Initialize() {
           AddView<LogoutView, LogoutViewModel>();
        }
    }
}
