using AppCommon.Generic;
using System.ComponentModel;
using MyUtilities;
using System.Collections.ObjectModel;
using System.Windows.Data;
using AppCommon.Views;
using System.Windows.Controls;
using SharedObjects;

namespace AppCommon.ViewModels
{
    public class CommonInfoViewModel : BaseViewModel {

        public CollectionViewSource                         ProccessInfoSource  { get; set; }
        public ObservableCollection<BindableProcessInfo>    ProccessInfo        { get; set; }

        public CollectionViewSource                         GameSource          { get; set; }
        public ObservableCollection<BindableGameInfo>       CurrentGame         { get; set; }
        
        private DataGrid CurrentGameGrid {
            get { return AppDispatcher.GetView<CommonInfoView>("CommonInfoRegion").CurrentGameGrid; }
        }

        public CommonInfoViewModel(){
            AppDispatcher.DispatchUI( () => {

                ProccessInfoSource  = new CollectionViewSource();
                GameSource          = new CollectionViewSource();
                ProccessInfo        = new ObservableCollection<BindableProcessInfo>();
                CurrentGame         = new ObservableCollection<BindableGameInfo>();

                ProccessInfoSource.Source = ProccessInfo;
                GameSource.Source         = CurrentGame;
                ProccessInfo.Add( AppState.Connection.Process.State.ProcessInfo  );
            });
            AppState.Connection.Process.State.ProcessInfo.PropertyChanged += new PropertyChangedEventHandler(ChangeState);
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            AppDispatcher.DispatchUI( () => {
                if(e.PropertyName == "Status") {
                    switch ( AppState.Connection.Process.State.Status ) {
                        case ProcessInfo.StatusCode.JoinedGame: case ProcessInfo.StatusCode.PlayingGame: case ProcessInfo.StatusCode.Won:
                        case ProcessInfo.StatusCode.Lost:       case ProcessInfo.StatusCode.Terminating: case ProcessInfo.StatusCode.Tied: 
                            CurrentGameGrid.Visibility = System.Windows.Visibility.Visible; break; // For all of the above cases
                        default: 
                           CurrentGameGrid.Visibility = System.Windows.Visibility.Hidden;  break; // Default, when not in game or entering/exiting a game.
                    }
                }
                else if( e.PropertyName == "CurrentGame") { 
                    CurrentGame.Clear();
                    CurrentGame.Add( AppState.Connection.Process.State.CurrentGame );
                }
            });
        }
    }
}
