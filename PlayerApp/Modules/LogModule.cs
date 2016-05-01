using PlayerApp.Views;
using Prism.Modularity;
using Prism.Regions;
using PlayerApp.ViewModels;
using System.Windows.Controls;
using PlayerApp.Generic;

namespace PlayerApp.Modules
{
    class LogModule : BaseModule {
        public LogModule(IRegionManager regionManager) : base(regionManager) {}

        override public void Initialize() {
           AddView<LogView, LogViewModel>("LogRegion");
        }
    }
}
