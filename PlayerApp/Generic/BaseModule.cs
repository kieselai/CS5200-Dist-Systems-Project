using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using PlayerApp.Views;
using Prism.Modularity;
using PlayerApp.ViewModels;
using System.Windows.Controls;


namespace PlayerApp.Generic
{
    public abstract class BaseModule : IModule {
        IRegionManager RegionManager { get; set; }

        public BaseModule(IRegionManager regionManager) {
            RegionManager = regionManager;
        }
        abstract public void Initialize();

        public void AddView<T, T2>(string region) where T : UserControl, new() where T2 : new() {
            var view = new T();
            view.DataContext = new T2();
            RegionManager.Regions[region].Add(view, AppState.ViewName<T>());
        }
        public void AddViewWithModel<T, T2>(string region, T2 viewModel) where T : UserControl, new() where T2 : new() {
            var view = new T();
            view.DataContext = viewModel;
            RegionManager.Regions[region].Add(view, AppState.ViewName<T>());
        }
    }
}