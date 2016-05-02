using System.Collections.Concurrent;
using SharedObjects;
using CommunicationLayer;
using MyUtilities;
using System.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace BalloonStoreProcess
{
    public class BalloonStoreState : ProcessState {

        public BalloonStoreState() : base() {
            Pennies = new ResourceSet<Penny>();
            Balloons = new ResourceSet<Balloon>();
            FilledBalloons = new ResourceSet<Balloon>();
            ProcessInfo.Info = new ProcessInfo {
                Status = SharedObjects.ProcessInfo.StatusCode.NotInitialized,
                Type   = SharedObjects.ProcessInfo.ProcessType.BalloonStore
            };
        }

        private int _gameManagerId;
        public int GameManagerId {
            get {  return _gameManagerId; }
            set { SetProperty(ref _gameManagerId, value); }
        }
        private int _gameId;
        public int GameId {
            get {  return _gameManagerId; }
            set { SetProperty(ref _gameManagerId, value); }
        }
        private int _startingBalloons;
        public int StartingBalloons {
            get {  return _gameManagerId; }
            set { SetProperty(ref _gameManagerId, value); }
        }

        private BindableGameProcessData _myBalloonStore;
        public BindableGameProcessData MyBalloonStore {
            get { return _myBalloonStore; }
            set { SetProperty( _myBalloonStore, value, (m) => _myBalloonStore.GameProcessData = value ); }
        }
        
        public ResourceSet<Penny>    Pennies        { get; set; }
        public ResourceSet<Balloon>  Balloons       { get; set; }
        public ResourceSet<Balloon>  FilledBalloons { get; set; }
        public bool? LoggedOut                      { get; set; }

        public override void Reset() {
            Pennies = new ResourceSet<Penny>();
            Balloons = new ResourceSet<Balloon>();
            CurrentGame = new GameInfo();
            FilledBalloons = new ResourceSet<Balloon>();
            MyBalloonStore = new MyUtilities.BindableGameProcessData();
        }
    }
}
