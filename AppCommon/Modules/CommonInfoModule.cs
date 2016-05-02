using AppCommon.Views;
using Prism.Regions;
using AppCommon.ViewModels;
using AppCommon.Generic;

namespace AppCommon.Modules
{
    class CommonInfoModule : BaseModule {
        public CommonInfoModule(IRegionManager regionManager) : base(regionManager) {
            RegionName = "CommonInfoRegion";
        }

        override public void Initialize() {
           AddView<CommonInfoView, CommonInfoViewModel>();
        }
    }
}
