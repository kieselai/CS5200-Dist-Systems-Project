using AppCommon.Generic;
using AppCommon;
using MyUtilities;
using System.Collections.ObjectModel;
using System.Windows.Data;
using PlayerProcess;

namespace Player.ViewModels
{
    public class OwnInfoViewModel : BaseViewModel {

        public CollectionViewSource                         IdentitySource      { get; set; }
        public ObservableCollection<BindableIdentityInfo>   Identity            { get; set; }

        public CollectionViewSource                         InGameInfoSource    { get; set; }
        public ObservableCollection<PlayerState>            InGameInfo          { get; set; }
        
        public OwnInfoViewModel() {
            AppDispatcher.DispatchUI( () => {
                IdentitySource   = new CollectionViewSource();
                InGameInfoSource = new CollectionViewSource();

                Identity         = new ObservableCollection<BindableIdentityInfo>();
                InGameInfo       = new ObservableCollection<PlayerState>();

                IdentitySource.Source   = Identity;
                InGameInfoSource.Source = InGameInfo;

                InGameInfo.Add( AppState.Launcher.Process.State as PlayerState );
                Identity.Add( AppState.Launcher.Process.State.IdentityInfo );
            });
        }
    }
}