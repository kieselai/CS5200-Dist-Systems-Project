using Prism.Regions;
using PlayerApp.ViewModels;
using AppCommon.Generic;
using AppCommon.Views;
namespace PlayerApp.Modules
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
