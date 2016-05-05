using AppCommon.ViewModels;
using Player.Views;
using AppCommon;
using AppCommon.Views;
using System;

namespace Player.ViewModels
{
    public class MainWindowViewModel : AbstractMainWindowViewModel {
        public override void initialize() {
            base.initialize();
            AppDispatcher.Navigate<LoginView>  ("MainRegion");
            AppDispatcher.Navigate<OwnInfoView>("OwnInfoRegion");
            AppDispatcher.DispatchUI(()=> {
                AppDispatcher.GetView<CommonInfoView>("CommonInfoRegion").Visibility = System.Windows.Visibility.Hidden;
                AppDispatcher.GetView<OwnInfoView>("OwnInfoRegion").Visibility = System.Windows.Visibility.Hidden;
            });
            if (Environment.GetCommandLineArgs().Length > 1) {
                LoginViewModel.ExecuteLogin();
            }
        }
    }
}