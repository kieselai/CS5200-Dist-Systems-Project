using AppCommon.Generic;
using AppCommon;
using MyUtilities;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Player;

namespace PlayerApp.ViewModels
{
    public class OwnInfoViewModel : BaseViewModel {

        public CollectionViewSource                         IdentitySource      { get; set; }
        public ObservableCollection<BindableIdentityInfo>   Identity            { get; set; }

        public CollectionViewSource                         InGameInfoSource    { get; set; }
        public ObservableCollection<PlayerState>            InGameInfo          { get; set; }
        
        private PlayerState PlayerState { get { return AppState.Connection.Process.State as PlayerState; } }

        public OwnInfoViewModel() {
            AppDispatcher.DispatchUI( () => {
                IdentitySource   = new CollectionViewSource();
                InGameInfoSource = new CollectionViewSource();

                Identity         = new ObservableCollection<BindableIdentityInfo>();
                InGameInfo       = new ObservableCollection<PlayerState>();

                IdentitySource.Source   = Identity;
                InGameInfoSource.Source = InGameInfo;

                InGameInfo.Add( PlayerState );
                Identity.Add( PlayerState.IdentityInfo );
            });
        }
    }
}