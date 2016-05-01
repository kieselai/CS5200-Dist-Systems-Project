using Microsoft.Practices.ServiceLocation;
using PlayerApp.Views;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using MyUtilities;
using System.Windows;
using System.Windows.Threading;

namespace PlayerApp.Generic
{
    abstract public class BaseViewModel : BindableEventObject {

        protected string _title;
        public string Title {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public BaseViewModel(string title) { Title = title; }
        protected IRegionManager RegionManagerInstance { get { return ServiceLocator.Current.GetInstance<IRegionManager>(); } }

        public MainWindow GetWindow(){
            return AppState.Container.Resolve<MainWindow>();
        }

        public ViewType GetView<ViewType>(string region) where ViewType : class {
            return RegionManagerInstance.Regions[region].GetView(AppState.ViewName<ViewType>()) as ViewType;
        }

        public ViewModelType GetViewModel<ViewType, ViewModelType>(string region) where ViewType : ContentControl where ViewModelType : BaseViewModel {
            var view = GetView<ViewType>(region);
            if( view == null ) return null;
            return view.DataContext as ViewModelType;
        }

        public void DispatchUI(Action UIAction) {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, UIAction);
        }

        public void Navigate<ViewType>(string region) {
            RegionManagerInstance.RequestNavigate(region, new Uri(AppState.ViewName<ViewType>(), UriKind.Relative));
        }

        public void Navigate<ViewType>(string region, Action<object> afterNavigate ) where ViewType : class {
            Navigate<ViewType>(region);
            var view = GetView<ViewType>(region);
            afterNavigate(view);
        }

        public DelegateCommand TrueDelegateCommand( Action action ) {
            return new DelegateCommand(action, () => { return true; });
        }

        public DelegateCommand<T> TrueDelegateCommand<T>( Action<T> action ) {
            return new DelegateCommand<T>(action, (t) => { return true; });
        }

        public DelegateCommand UnconditionalRedirect<T>(string section) {
            return TrueDelegateCommand( () => { Navigate<T>(section); });
        }

        public DelegateCommand UnconditionalRedirect<T>(string section, Action<object> afterRedirect) where T : class {
            return TrueDelegateCommand( () => { Navigate<T>(section, afterRedirect); });
        }
}
}