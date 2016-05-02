using PlayerApp.Views;
using Prism.Regions;
using PlayerApp.ViewModels;
using AppCommon.Generic;
namespace PlayerApp.Modules
{
    class OwnInfoModule : BaseModule {
        public OwnInfoModule(IRegionManager regionManager) : base(regionManager) {
            RegionName = "OwnInfoRegion";
        }

        override public void Initialize() {
           AddView<OwnInfoView, OwnInfoViewModel>();
        }
    }
}
