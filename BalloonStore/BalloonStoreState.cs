using System.Collections.Concurrent;
using SharedObjects;
using CommunicationLayer;
using MyUtilities;
using System.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace BalloonStore
{
    public class BalloonStoreState : ProcessState {

        public BalloonStoreState() : base() {
            _currGame     = new GameInfo();
            _myBalloonStore     = new BindableGameProcessData();
            Pennies = new ResourceSet<Penny>();
            Balloons = new ResourceSet<Balloon>();
            FilledBalloons = new ResourceSet<Balloon>();
            ProcessInfo.Info = new ProcessInfo {
                Status = SharedObjects.ProcessInfo.StatusCode.NotInitialized,
                Type   = SharedObjects.ProcessInfo.ProcessType.BalloonStore
            };
        }

        public ProcessInfo.StatusCode Status {
            get { return ProcessInfo.Status;  }
        }

        private BindableGameInfo _currGame;
        public  BindableGameInfo CurrentGame {
            get { return _currGame; }
            set { SetProperty( _currGame, value, (c)=> _currGame.Info = value); }
        }

        private BindableGameProcessData _myBalloonStore;
        public BindableGameProcessData MyBalloonStore {
            get { return _myBalloonStore; }
            set { SetProperty( _myBalloonStore, value, (m) => _myBalloonStore.GameProcessData = value ); }
        }
        
        public ConcurrentQueue<GameInfo> OpenGames  { get; set; }
        public ResourceSet<Penny>    Pennies        { get; set; }
        public ResourceSet<Balloon>  Balloons       { get; set; }
        public ResourceSet<Balloon>  FilledBalloons { get; set; }
        public bool? LoggedOut                      { get; set; }
    }
}
