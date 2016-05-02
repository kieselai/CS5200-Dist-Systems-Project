using Microsoft.Practices.ServiceLocation;
using AppCommon.Views;
using Prism.Regions;
using System;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using System.Windows;
using System.Windows.Threading;
using AppCommon.Generic;

namespace AppCommon
{
    public static class AppDispatcher {

        public static ViewType GetView<ViewType>(string region) where ViewType : class {
            return AppState.RegionManager.Regions[region].GetView(AppState.ViewName<ViewType>()) as ViewType;
        }

        public static ViewModelType GetViewModel<ViewType, ViewModelType>(string region) where ViewType : ContentControl where ViewModelType : BaseViewModel {
            var view = GetView<ViewType>(region);
            if( view == null ) return null;
            return view.DataContext as ViewModelType;
        }

        public static void DispatchUI(Action UIAction) {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, UIAction);
        }

        public static void Navigate<ViewType>(this IRegionManager regionManager, string region, Action<object> afterNavigate=null) where ViewType : class {
            regionManager.RequestNavigate(region, new Uri(AppState.ViewName<ViewType>(), UriKind.Relative));
            if(afterNavigate != null) {
                var view = GetView<ViewType>(region);
                afterNavigate(view);
            }
        }

        public static void Navigate<ViewType>(string region, Action<object> afterNavigate=null) where ViewType : class {
            AppState.RegionManager.Navigate<ViewType>(region, afterNavigate);
        }
    }
}