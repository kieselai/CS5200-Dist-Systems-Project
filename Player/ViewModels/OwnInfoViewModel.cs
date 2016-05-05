using AppCommon.Generic;
using AppCommon;
using MyUtilities;
using System.Collections.ObjectModel;
using System.Windows.Data;
using PlayerProcess;
using System.ComponentModel;

namespace Player.ViewModels
{
    public class OwnInfoViewModel : BaseViewModel {

        public CollectionViewSource                         IdentitySource      { get; set; }
        public CollectionViewSource                         InGameInfoSource    { get; set; }

        public ObservableCollection<BindableIdentityInfo>   Identity            { get; set; }

        
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
                 AppState.Launcher.Process.State.PropertyChanged += new PropertyChangedEventHandler(OnUpdate);
            });
        }

        public void OnUpdate(object sender, PropertyChangedEventArgs e) {
            AppDispatcher.DispatchUI( () => {
                var changeOccured = false;
                Chain.Create("InitialLifePoints", "HitPoints", "CurrentLifePoints", "NumberOfPennies", 
                    "NumberOfUnfilledBalloons", "NumberOfFilledBalloons", "NumberOfUmbrellas")
                    .Tap( (prop)=> changeOccured = e.PropertyName == prop? true : changeOccured);
                if(changeOccured) {
                    InGameInfo.Clear();
                    InGameInfo.Add( AppState.Launcher.Process.State as PlayerState );
                }
            });
        }
    }
}