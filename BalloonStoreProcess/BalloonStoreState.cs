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
            get {  return _gameId; }
            set { SetProperty(ref _gameId, value); }
        }
        private int _startingBalloons;
        public int StartingBalloons {
            get {  return _startingBalloons; }
            set { SetProperty(ref _startingBalloons, value); }
        }
        private int _storeIndex;
        public int StoreIndex {
            get {  return _storeIndex; }
            set { SetProperty(ref _storeIndex, value); }
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
        }

        public override string GetMessageFromStatus(ProcessInfo.StatusCode status) {
            switch ( status ) {
                case SharedObjects.ProcessInfo.StatusCode.Registered:     return "Registered, Preparing Balloons";
                case SharedObjects.ProcessInfo.StatusCode.PlayingGame:    return "In game";
                default: return GetDefaultMessageFromStatus(status);
            }
        }
    }
}
