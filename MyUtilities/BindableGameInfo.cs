using SharedObjects;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyUtilities
{
    public class BindableGameInfo : BindableEventObject {

        public GameInfo Info {
            get {
                return new GameInfo {
                    CurrentProcesses = CurrentProcesses.Select((d) => (GameProcessData)d).ToArray(),
                    GameId           = GameId,
                    GameManagerId    = GameManagerId,
                    Label            = Label,
                    MinPlayers       = MinPlayers,
                    MaxPlayers       = MaxPlayers,
                    StartingPlayers  = StartingPlayers,
                    Status           = Status,
                    Winners          = Winners
                };
            }
            set {
                CurrentProcesses = new ObservableCollection<BindableGameProcessData>(
                    value.CurrentProcesses.Select( (d) => (BindableGameProcessData)d).ToList() );
                GameId           = value.GameId;
                GameManagerId    = value.GameManagerId;
                Label            = value.Label;
                MinPlayers       = value.MinPlayers;
                MaxPlayers       = value.MaxPlayers;
                StartingPlayers  = value.StartingPlayers;
                Status           = value.Status;
                Winners          = value.Winners;
            }
        }

        public BindableGameInfo() {}
        public BindableGameInfo(GameInfo info)         {
             _currentProcesses = new ObservableCollection<BindableGameProcessData>();
            Info = info;
        }
        public BindableGameInfo(BindableGameInfo info) {
            Info = info.Info;
             _currentProcesses = new ObservableCollection<BindableGameProcessData>();
        }

        public void Reset() {
            Info = new GameInfo {
                CurrentProcesses = new GameProcessData[]{},
                StartingPlayers = new int[]{},
                Status = GameInfo.StatusCode.NotInitialized,
                Winners = new int[]{}, 
                GameId = 0,
                GameManagerId=0,
                Label="",
                MaxPlayers=0,
                MinPlayers=0
            };
        }

        public static implicit operator GameInfo(BindableGameInfo info) {
            return info.Info;
        }

        public static implicit operator BindableGameInfo(GameInfo info) {
            return new BindableGameInfo(info);
        }

        private int _gameId;
        public  int GameId {
            get { return _gameId; }
            set { SetProperty( ref _gameId, value); }
        }

        private int _gameManagerId;
        public  int GameManagerId {
            get { return _gameManagerId; }
            set { SetProperty( ref _gameManagerId, value ); }
        }
        
        private  ObservableCollection<BindableGameProcessData> _currentProcesses;
        public   ObservableCollection<BindableGameProcessData> CurrentProcesses {
            get { return _currentProcesses; }
            set {
                _currentProcesses.Tap((orig)=> {
                    var matching = value.Where(p1 => p1.ProcessId == orig.ProcessId);
                    if( matching.Count() == 0 ) {
                        _currentProcesses.Remove(orig);
                    }
                    else matching.Tap((p1)=> orig = p1);
                });
            }
        }

        private string _label;
        public  string Label {
            get { return _label; }
            set { SetProperty( ref _label, value ); }
        }
        private int _minPlayers;
        public  int MinPlayers {
            get { return _minPlayers; }
            set { SetProperty( ref _minPlayers, value); }
        }
        private int _maxPlayers;
        public  int MaxPlayers {
            get { return _maxPlayers; }
            set { SetProperty( ref _maxPlayers, value); }
        }
        private int[] _startingPlayers;
        public  int[] StartingPlayers {
            get { return _startingPlayers; }
            set { SetProperty( ref _startingPlayers,  value ); }
        }

        private GameInfo.StatusCode _status;
        public  GameInfo.StatusCode Status {
            get { return _status; }
            set { SetProperty( ref _status, value); }
        }

        private int[] _winners;
        public  int[] Winners {
            get { return _winners; }
            set { SetProperty( ref _winners,  value); }
        }
    }
}