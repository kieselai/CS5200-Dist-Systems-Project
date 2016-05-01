using System.Collections.Concurrent;
using SharedObjects;
using CommunicationLayer;
using MyUtilities;
using System.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace Player
{
    public class PlayerState : ProcessState {

        public PlayerState() : base() {
            _identityInfo = new BindableIdentityInfo();
            _currGame     = new GameInfo();
            _myPlayer     = new BindableGameProcessData();
            IdentityInfo.PropertyChanged += new PropertyChangedEventHandler(OnIdentityInfoChanged);
            Pennies = new ResourceSet<Penny>();
            Balloons = new ResourceSet<Balloon>();
            FilledBalloons = new ResourceSet<Balloon>();
            ProcessInfo.Info = new ProcessInfo {
                Status = SharedObjects.ProcessInfo.StatusCode.NotInitialized,
                Type   = SharedObjects.ProcessInfo.ProcessType.Player
            };
        }

        public ProcessInfo.StatusCode Status {
            get { return ProcessInfo.Status;  }
        }

        protected void OnIdentityInfoChanged(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "Alias") {
                ProcessInfo.Label = IdentityInfo.Alias;
            }
        }

        private BindableIdentityInfo _identityInfo;
        public  BindableIdentityInfo IdentityInfo {
            get { return _identityInfo; }
            set { SetProperty( _identityInfo, value, (i) => _identityInfo.Info = value ); }
        }

        private BindableGameInfo _currGame;
        public  BindableGameInfo CurrentGame {
            get { return _currGame; }
            set { SetProperty( _currGame, value, (c)=> _currGame.Info = value); }
        }

        private BindableGameProcessData _myPlayer;
        public BindableGameProcessData MyPlayer {
            get { return _myPlayer; }
            set { SetProperty( _myPlayer, value, (m) => _myPlayer.GameProcessData = value ); }
        }
        
        public ConcurrentQueue<GameInfo> OpenGames  { get; set; }
        public ResourceSet<Penny>    Pennies        { get; set; }
        public ResourceSet<Balloon>  Balloons       { get; set; }
        public ResourceSet<Balloon>  FilledBalloons { get; set; }
        public bool? LoggedOut                      { get; set; }

        private int _initialLifePoints;
        public  int InitialLifePoints {
            get { return _initialLifePoints; }
            set { SetProperty( ref _initialLifePoints, value); OnPropertyChanged("CurrentLifePoints"); }
        }
        private int _hitPoints;
        public  int  HitPoints {
            get { return _hitPoints; }
            set { SetProperty( ref _hitPoints, value);         OnPropertyChanged("CurrentLifePoints"); }
        }
        public int  CurrentLifePoints { get { return InitialLifePoints - HitPoints; } }
    }
}
