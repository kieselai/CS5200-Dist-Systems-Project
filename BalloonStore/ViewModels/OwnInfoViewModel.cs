using AppCommon.Generic;
using AppCommon;
using MyUtilities;
using System.Collections.ObjectModel;
using System.Windows.Data;
using BalloonStore;
using BalloonStoreProcess;

namespace BalloonStore.ViewModels
{
    //public class OwnInfoViewModel : BaseViewModel {

    //    public CollectionViewSource                         InGameInfoSource    { get; set; }
    //    public ObservableCollection<BalloonStoreState>      InGameInfo          { get; set; }
        
    //    private BalloonStoreState BalloonStoreState { get { return AppState.Launcher.Process.State as BalloonStoreState; } }

    //    public OwnInfoViewModel() {
    //        AppDispatcher.DispatchUI( () => {
    //            InGameInfoSource = new CollectionViewSource();
    //            InGameInfo       = new ObservableCollection<BalloonStoreState>();
    //            InGameInfoSource.Source = InGameInfo;
    //            InGameInfo.Add( BalloonStoreState );
    //        });
    //    }
    //}

    public class OwnInfoViewModel : BaseViewModel {

        public CollectionViewSource                         IdentitySource      { get; set; }
        public ObservableCollection<BindableIdentityInfo>   Identity            { get; set; }
        
        public OwnInfoViewModel() {
            AppDispatcher.DispatchUI( () => {
                IdentitySource   = new CollectionViewSource();
                Identity         = new ObservableCollection<BindableIdentityInfo>();
                IdentitySource.Source   = Identity;
                Identity.Add( AppState.Launcher.Process.State.IdentityInfo );
            });
        }
    }
}