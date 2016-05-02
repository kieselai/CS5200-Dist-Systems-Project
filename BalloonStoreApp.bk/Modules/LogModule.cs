using BalloonStoreApp.Views;
using Prism.Modularity;
using Prism.Regions;
using BalloonStoreApp.ViewModels;
using System.Windows.Controls;
using BalloonStoreApp.Generic;

namespace BalloonStoreApp.Modules
{
    class LogModule : BaseModule {
        public LogModule(IRegionManager regionManager) : base(regionManager) {}

        override public void Initialize() {
           AddView<LogView, LogViewModel>("LogRegion");
        }
    }
}
