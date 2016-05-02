using BalloonStoreApp.Generic;
using System.ComponentModel;
using MyUtilities;
using System.Collections.ObjectModel;
using System.Windows.Data;
using BalloonStoreApp.Views;
using System.Windows.Controls;
using BalloonStore;
using SharedObjects;
using System.Linq;
using System;

namespace BalloonStoreApp.ViewModels
{
    public class StatusViewModel : BaseViewModel {

        public CollectionViewSource                         MyBalloonStoreSource      { get; set; }
        public ObservableCollection<BindableProcessInfo>    MyBalloonStoreStats       { get; set; }

        public CollectionViewSource                         IdentitySource      { get; set; }
        public ObservableCollection<BindableIdentityInfo>   Identity            { get; set; }

        public CollectionViewSource                         GameSource          { get; set; }
        public ObservableCollection<BindableGameInfo>       CurrentGame         { get; set; }

        
        public CollectionViewSource                         InGameBalloonStoreSource  { get; set; }
        public ObservableCollection<BalloonStoreState>            InGameBalloonStore        { get; set; }
        
        private BalloonStoreState BalloonStoreState { get { return AppState.Connection.BalloonStore.BalloonStoreState; } }

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

                MyBalloonStoreSource      = new CollectionViewSource();
                IdentitySource      = new CollectionViewSource();
                GameSource          = new CollectionViewSource();
                InGameBalloonStoreSource  = new CollectionViewSource();

                MyBalloonStoreStats  = new ObservableCollection<BindableProcessInfo>();
                Identity       = new ObservableCollection<BindableIdentityInfo>();
                CurrentGame    = new ObservableCollection<BindableGameInfo>();
                InGameBalloonStore    = new ObservableCollection<BalloonStoreState>();

                MyBalloonStoreSource.Source     = MyBalloonStoreStats;
                IdentitySource.Source     = Identity;
                GameSource.Source         = CurrentGame;
                InGameBalloonStoreSource.Source = InGameBalloonStore;

                InGameBalloonStore.Add( BalloonStoreState);
                MyBalloonStoreStats.Add( BalloonStoreState.ProcessInfo  );
                
            });
            BalloonStoreState.PropertyChanged += new PropertyChangedEventHandler(ChangeState);
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            DispatchUI( () => {
                if(e.PropertyName == "Status") {
                    switch ( BalloonStoreState.Status ) {
                        case ProcessInfo.StatusCode.JoinedGame: case ProcessInfo.StatusCode.PlayingGame: case ProcessInfo.StatusCode.Won:
                        case ProcessInfo.StatusCode.Lost:       case ProcessInfo.StatusCode.Terminating: case ProcessInfo.StatusCode.Tied: 
                            CurrentGameGrid.Visibility = System.Windows.Visibility.Visible; break; // For all of the above cases
                            
                        default: 
                           CurrentGameGrid.Visibility = System.Windows.Visibility.Hidden;  break; // Default, when not in game or entering/exiting a game.
                    }
                }
                else if( e.PropertyName == "CurrentGame") { 
                    CurrentGame.Clear();
                    CurrentGame.Add( BalloonStoreState.CurrentGame );
                }
                else if( e.PropertyName == "InitialLifePoints" || e.PropertyName == "InitialLifePoints" || e.PropertyName == "HitPoints") {
                    InGameBalloonStore.Clear();
                    InGameBalloonStore.Add( BalloonStoreState );
                }
            });
        }
    }
}
