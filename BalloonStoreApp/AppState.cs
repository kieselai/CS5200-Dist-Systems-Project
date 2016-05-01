using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUtilities;
using BalloonStore;
using Microsoft.Practices.Unity;
using BalloonStoreApp.ViewModels;
using BalloonStoreApp.Views;

namespace BalloonStoreApp {
    public static class AppState {
        static public Bootstrapper           Bootstrapper  { get; set; }
        static public BalloonStoreConnection Connection    { get; set; }
        static public EventAppender          EventAppender { get; set; }
        static public IUnityContainer        Container     { get { return Bootstrapper.Container; } }

        private static readonly Dictionary<Type, Type> _viewModelMapping = new Dictionary<Type, Type> {
            { typeof(LoginView),   typeof(LoginViewModel)   },
            { typeof(MessageView), typeof(MessageViewModel) }
        };

        public static Type ViewModelMapping<ViewType>() {
            return _viewModelMapping[typeof(ViewType)];
        }

        public static string ViewName<ViewType>() {
            return typeof(ViewType).FullName;
        }
    }
}
