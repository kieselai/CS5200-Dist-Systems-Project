using AppCommon.Views;
using Prism.Regions;
using AppCommon.ViewModels;
using AppCommon.Generic;

namespace AppCommon.Modules
{
    public class LogModule : BaseModule {
        public LogModule(IRegionManager regionManager) : base(regionManager) {
            RegionName = "LogRegion";
        }
        override public void Initialize() {
           AddView<LogView, LogViewModel>();
        }
    }
}
