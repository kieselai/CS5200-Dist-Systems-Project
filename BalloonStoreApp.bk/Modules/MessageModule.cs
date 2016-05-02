using BalloonStoreApp.Views;
using Prism.Regions;
using BalloonStoreApp.ViewModels;
using BalloonStoreApp.Generic;

namespace BalloonStoreApp.Modules
{
    class MessageModule : BaseModule {
        public MessageModule(IRegionManager regionManager) : base(regionManager) {}

        override public void Initialize() {
           AddView<MessageView, MessageViewModel>("MessageRegion");
        }
    }
}
