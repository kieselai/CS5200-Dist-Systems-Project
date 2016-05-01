using PlayerApp.Views;
using Prism.Regions;
using PlayerApp.ViewModels;
using PlayerApp.Generic;

namespace PlayerApp.Modules
{
    class MessageModule : BaseModule {
        public MessageModule(IRegionManager regionManager) : base(regionManager) {}

        override public void Initialize() {
           AddView<MessageView, MessageViewModel>("MessageRegion");
        }
    }
}
