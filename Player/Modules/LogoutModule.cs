using Prism.Regions;
using Player.ViewModels;
using AppCommon.Generic;
using AppCommon.Views;
namespace Player.Modules
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
