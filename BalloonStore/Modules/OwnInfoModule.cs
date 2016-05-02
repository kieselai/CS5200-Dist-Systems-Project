using BalloonStore.Views;
using Prism.Regions;
using BalloonStore.ViewModels;
using AppCommon.Generic;
namespace BalloonStore.Modules
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
