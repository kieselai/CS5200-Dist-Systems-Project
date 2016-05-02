using AppCommon.Views;
using Prism.Regions;
using AppCommon.ViewModels;
using AppCommon.Generic;

namespace AppCommon.Modules
{
    public class MessageModule : BaseModule {
        public MessageModule(IRegionManager regionManager) : base(regionManager) {
            RegionName = "MessageRegion";
        }

        override public void Initialize() {
           AddView<MessageView, MessageViewModel>();
        }
    }
}