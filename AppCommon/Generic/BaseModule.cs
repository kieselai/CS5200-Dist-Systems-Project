using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using AppCommon.Views;
using Prism.Modularity;
using System.Windows.Controls;


namespace AppCommon.Generic
{
    public abstract class BaseModule : IModule {
        protected IRegionManager RegionManager { get; set; }
        protected string         RegionName    { get; set; }
        public    IRegion        Region        { get { return RegionManager.Regions[RegionName]; } }

        public BaseModule(IRegionManager regionManager) {
            RegionManager = regionManager;
        }

        abstract public void Initialize();

        public IRegionManager AddView<T, T2>(T2 viewModel=null, bool ownRegionManager=false) where T : UserControl, new() where T2 : BaseViewModel, new() {
            T view = null;
            return AddView(out view, viewModel, ownRegionManager);
        }

        public IRegionManager AddView<T, T2>(out T view, T2 viewModel=null, bool ownRegionManager=false) where T : UserControl, new() where T2 : BaseViewModel, new() {
            view = new T();
            view.DataContext = viewModel?? new T2();
            if(ownRegionManager) return Region.Add(view, AppState.ViewName<T>(), ownRegionManager);
            else return Region.Add(view, AppState.ViewName<T>(), ownRegionManager);
        }
        public IRegionManager AddView<T, T2>(out T view, out T2 viewModel, bool ownRegionManager=false) where T : UserControl, new() where T2 : BaseViewModel, new() {
            view = new T();
            viewModel = new T2();
            view.DataContext = viewModel;
            if(ownRegionManager) return Region.Add(view, AppState.ViewName<T>(), ownRegionManager);
            else return Region.Add(view, AppState.ViewName<T>(), ownRegionManager);
        }
        public IRegionManager AddView<T, T2>(out T2 viewModel, bool ownRegionManager=false) where T : UserControl, new() where T2 : BaseViewModel, new() {
            var view = new T();
            viewModel = new T2();
            view.DataContext = viewModel;
            if(ownRegionManager) return Region.Add(view, AppState.ViewName<T>(), ownRegionManager);
            else return Region.Add(view, AppState.ViewName<T>(), ownRegionManager);
        }
    }
}