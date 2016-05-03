using System.Collections.Concurrent;
using SharedObjects;
using CommunicationLayer;
using MyUtilities;
using System.ComponentModel;

namespace PlayerProcess
{
    public class PlayerState : ProcessState {

        public PlayerState() : base() {
            _myPlayer     = new BindableGameProcessData();
            Pennies = new ResourceSet<Penny>();
            Balloons = new ResourceSet<Balloon>();
            FilledBalloons = new ResourceSet<Balloon>();
            ProcessInfo.Info = new ProcessInfo {
                Status = SharedObjects.ProcessInfo.StatusCode.NotInitialized,
                Type   = SharedObjects.ProcessInfo.ProcessType.Player
            };
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

        public override void Reset() {
            base.Reset();
            Pennies = new ResourceSet<Penny>();
            Balloons = new ResourceSet<Balloon>();
            FilledBalloons = new ResourceSet<Balloon>();
            HitPoints = 0;
            InitialLifePoints = 0;
            MyPlayer = new BindableGameProcessData();
            OpenGames = null;
        }
        
    }
}
