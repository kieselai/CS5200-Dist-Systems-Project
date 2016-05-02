using System;
using System.Collections.Generic;
using System.Linq;
using MyUtilities;
using ProcessCommon;
using AppCommon.ViewModels;
using AppCommon.Views;
using Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace AppCommon
{
    public class AppState {
        static private AppState       _instance;
        static private AppState       SetInstance() {  _instance = new AppState();   return _instance; }
        static public  AppState       Instance      { get { return _instance?? SetInstance(); } }
        private AppState() {}

        public static  IRegionManager RegionManager { get { return ServiceLocator.Current.GetInstance<IRegionManager>(); } }
        static public  BaseConnection Connection    { get; set; }
        static public  EventAppender  EventAppender { get; set; }

        public static string ViewName<ViewType>() {
            return typeof(ViewType).FullName;
        }
    }
}