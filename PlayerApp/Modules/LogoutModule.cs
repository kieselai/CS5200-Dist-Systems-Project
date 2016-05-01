using PlayerApp.Views;
using Prism.Modularity;
using Prism.Regions;
using PlayerApp.ViewModels;
using System.Windows.Controls;
using PlayerApp.Generic;

namespace PlayerApp.Modules
{
    class LogoutModule : BaseModule {
        public LogoutModule(IRegionManager regionManager) : base(regionManager) {}

        override public void Initialize() {
           AddView<LogoutView, LogoutViewModel>("LogoutRegion");
        }
    }
}
