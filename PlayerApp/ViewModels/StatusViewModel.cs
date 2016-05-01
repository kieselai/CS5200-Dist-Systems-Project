using PlayerApp.Generic;
using System.ComponentModel;
using MyUtilities;
using System.Collections.ObjectModel;
using System.Windows.Data;
using PlayerApp.Views;
using System.Windows.Controls;
using Player;
using SharedObjects;
using System.Linq;
using System;

namespace PlayerApp.ViewModels
{
    public class StatusViewModel : BaseViewModel {

        public CollectionViewSource                         MyPlayerSource      { get; set; }
        public ObservableCollection<BindableProcessInfo>    MyPlayerStats       { get; set; }

        public CollectionViewSource                         IdentitySource      { get; set; }
        public ObservableCollection<BindableIdentityInfo>   Identity            { get; set; }

        public CollectionViewSource                         GameSource          { get; set; }
        public ObservableCollection<BindableGameInfo>       CurrentGame         { get; set; }

        
        public CollectionViewSource                         InGamePlayerSource  { get; set; }
        public ObservableCollection<PlayerState>            InGamePlayer        { get; set; }
        
        private PlayerState PlayerState { get { return AppState.Connection.Player.PlayerState; } }

        private StatusView View {
            get {  return GetView<StatusView>("MainRegion"); }
        }
        private DataGrid CurrentGameGrid {
            get { return View.CurrentGameGrid; }
        }

        public void setupDataGrid<T>(Action<ObservableCollection<T>> setupCollection, Action<CollectionViewSource> setupSource ) {

        }

        public StatusViewModel() : base("") {
            DispatchUI( () => {

                MyPlayerSource      = new CollectionViewSource();
                IdentitySource      = new CollectionViewSource();
                GameSource          = new CollectionViewSource();
                InGamePlayerSource  = new CollectionViewSource();

                MyPlayerStats  = new ObservableCollection<BindableProcessInfo>();
                Identity       = new ObservableCollection<BindableIdentityInfo>();
                CurrentGame    = new ObservableCollection<BindableGameInfo>();
                InGamePlayer    = new ObservableCollection<PlayerState>();

                MyPlayerSource.Source     = MyPlayerStats;
                IdentitySource.Source     = Identity;
                GameSource.Source         = CurrentGame;
                InGamePlayerSource.Source = InGamePlayer;

                InGamePlayer.Add( PlayerState);
                MyPlayerStats.Add( PlayerState.ProcessInfo  );
                     Identity.Add(PlayerState.IdentityInfo );
                
            });
            PlayerState.PropertyChanged += new PropertyChangedEventHandler(ChangeState);
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            DispatchUI( () => {
                if(e.PropertyName == "Status") {
                    switch ( PlayerState.Status ) {
                        case ProcessInfo.StatusCode.JoinedGame: case ProcessInfo.StatusCode.PlayingGame: case ProcessInfo.StatusCode.Won:
                        case ProcessInfo.StatusCode.Lost:       case ProcessInfo.StatusCode.Terminating: case ProcessInfo.StatusCode.Tied: 
                            CurrentGameGrid.Visibility = System.Windows.Visibility.Visible; break; // For all of the above cases
                            
                        default: 
                           CurrentGameGrid.Visibility = System.Windows.Visibility.Hidden;  break; // Default, when not in game or entering/exiting a game.
                    }
                }
                else if( e.PropertyName == "CurrentGame") { 
                    CurrentGame.Clear();
                    CurrentGame.Add( PlayerState.CurrentGame );
                }
                else if( e.PropertyName == "InitialLifePoints" || e.PropertyName == "InitialLifePoints" || e.PropertyName == "HitPoints") {
                    InGamePlayer.Clear();
                    InGamePlayer.Add( PlayerState );
                }
            });
        }
    }
}
