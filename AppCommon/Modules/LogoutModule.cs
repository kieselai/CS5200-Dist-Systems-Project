using Prism.Regions;
using AppCommon.ViewModels;
using AppCommon.Generic;
using AppCommon.Views;
namespace AppCommon.Modules
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
