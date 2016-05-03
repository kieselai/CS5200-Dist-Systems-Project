using Player.Views;
using Prism.Regions;
using Player.ViewModels;
using AppCommon.Generic;
namespace Player.Modules
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
