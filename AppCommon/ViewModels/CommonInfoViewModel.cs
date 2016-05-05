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

        public CollectionViewSource                         ProcessInfoSource  { get; set; }
        public ObservableCollection<BindableProcessInfo>    ProcessInfo        { get; set; }

        public CollectionViewSource                         GameSource          { get; set; }
        public ObservableCollection<BindableGameInfo>       CurrentGame         { get; set; }
        
        private DataGrid CurrentGameGrid {
            get { return AppDispatcher.GetView<CommonInfoView>("CommonInfoRegion").CurrentGameGrid; }
        }

        public CommonInfoViewModel(){
            AppDispatcher.DispatchUI( () => {

                ProcessInfoSource  = new CollectionViewSource();
                GameSource         = new CollectionViewSource();
                ProcessInfo        = new ObservableCollection<BindableProcessInfo>();
                CurrentGame        = new ObservableCollection<BindableGameInfo>();

                ProcessInfoSource.Source = ProcessInfo;
                GameSource.Source        = CurrentGame;
                ProcessInfo.Add( AppState.Launcher.Process.State.ProcessInfo  );
            });
            AppState.Launcher.Process.State.ProcessInfo.PropertyChanged += new PropertyChangedEventHandler(ChangeState);
            AppState.Launcher.Process.State.PropertyChanged += new PropertyChangedEventHandler(OnCurrentGameChange);            
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            AppDispatcher.DispatchUI( () => {
                if(e.PropertyName == "Status") {
                    switch ( AppState.Launcher.Process.State.Status ) {
                        case SharedObjects.ProcessInfo.StatusCode.JoinedGame:
                        case SharedObjects.ProcessInfo.StatusCode.PlayingGame:
                        case SharedObjects.ProcessInfo.StatusCode.Won:
                        case SharedObjects.ProcessInfo.StatusCode.Lost:
                        case SharedObjects.ProcessInfo.StatusCode.Terminating:
                        case SharedObjects.ProcessInfo.StatusCode.Tied: 
                            CurrentGameGrid.Visibility = System.Windows.Visibility.Visible; break; // For all of the above cases
                        default: 
                           CurrentGameGrid.Visibility = System.Windows.Visibility.Hidden;  break; // Default, when not in game or entering/exiting a game.
                    }
                }
            });
        }

        public void OnCurrentGameChange(object sender, PropertyChangedEventArgs e) {
            AppDispatcher.DispatchUI( () => {
                if( e.PropertyName == "CurrentGame") { 
                    CurrentGame.Clear();
                    CurrentGame.Add( AppState.Launcher.Process.State.CurrentGame );
                }
            });
        }
    }
}
