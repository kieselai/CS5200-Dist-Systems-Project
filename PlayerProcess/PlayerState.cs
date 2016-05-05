using System.Collections.Concurrent;
using SharedObjects;
using CommunicationLayer;
using MyUtilities;
using System.ComponentModel;

namespace PlayerProcess
{
    public class PlayerState : ProcessState {

        public PlayerState() : base() {
            Pennies        = new ResourceSet<Penny>();
            Balloons       = new ResourceSet<Balloon>();
            FilledBalloons = new ResourceSet<Balloon>();
            Umbrellas      = new ResourceSet<Umbrella>();
            ProcessInfo    = new ProcessInfo {
                Status = SharedObjects.ProcessInfo.StatusCode.NotInitialized,
                Type   = SharedObjects.ProcessInfo.ProcessType.Player
            };
            ThisGameProc.PropertyChanged += new PropertyChangedEventHandler(OnGameInfoChanged);
        }
        
        protected void OnGameInfoChanged(object sender, PropertyChangedEventArgs e) {
            HitPoints = ThisGameProc.HitPoints;
        }

        public ConcurrentQueue<GameInfo> OpenGames      { get; set; }

        public  int NumberOfPennies {  get { return _pennies.AvailableCount; } }
        private ResourceSet<Penny> _pennies;
        public ResourceSet<Penny> Pennies {
            get {
                ThreadUtil.RunAfterDelay(()=> OnPropertyChanged("NumberOfPennies"));
                return _pennies;
            }
            set { SetProperty(ref _pennies, value);  OnPropertyChanged("NumberOfPennies"); }
        }

        public  int NumberOfUnfilledBalloons {  get { return _balloons.AvailableCount; } }
        private ResourceSet<Balloon> _balloons;
        public ResourceSet<Balloon> Balloons {
            get {
                ThreadUtil.RunAfterDelay(()=> OnPropertyChanged("NumberOfUnfilledBalloons"));
                return _balloons;
            }
            set { SetProperty(ref _balloons, value); OnPropertyChanged("NumberOfUnfilledBalloons"); }
        }


        public  int NumberOfFilledBalloons {  get { return _filledBalloons.AvailableCount; } }
        private ResourceSet<Balloon> _filledBalloons;
        public  ResourceSet<Balloon> FilledBalloons {
            get {
                ThreadUtil.RunAfterDelay(()=> OnPropertyChanged("NumberOfFilledBalloons"));
                return _filledBalloons;
            }
            set { SetProperty(ref _filledBalloons, value); OnPropertyChanged("NumberOfFilledBalloons"); }
        }

        public  int NumberOfUmbrellas {  get { return _umbrellas.AvailableCount; } }
        private ResourceSet<Umbrella> _umbrellas;
        public  ResourceSet<Umbrella> Umbrellas {
            get {
                ThreadUtil.RunAfterDelay(()=> OnPropertyChanged("NumberOfUmbrellas"));
                return _umbrellas;
            }
            set { SetProperty(ref _umbrellas, value); OnPropertyChanged("NumberOfUmbrellas"); }
        }

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
            Pennies        = new ResourceSet<Penny>();
            Balloons       = new ResourceSet<Balloon>();
            FilledBalloons = new ResourceSet<Balloon>();
            HitPoints = 0;
            InitialLifePoints = 0;
            OpenGames = null;
        }

        public override string GetMessageFromStatus(ProcessInfo.StatusCode status) {
            switch ( status ) {
                case SharedObjects.ProcessInfo.StatusCode.Registered:     return "Registered, Retrieving game list";
                case SharedObjects.ProcessInfo.StatusCode.PlayingGame:    return "Playing ( In game )";
                default: return GetDefaultMessageFromStatus(status);
            }
        }
    }
}
