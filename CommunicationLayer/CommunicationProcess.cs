using SharedObjects;
using System;
using Utils;

namespace CommunicationLayer
{
    public abstract class CommunicationProcess : BackgroundThread {
        public CommunicationSubsystem  SubSystem  { get; set; }
        public ProcessState            State      { get; set; }

        public CommunicationProcess(ProcessState _state, ConversationFactory factory, int minPort, int maxPort) : base() {
            State = _state;
            SetStatus(ProcessInfo.StatusCode.NotInitialized);
            SubSystem = new CommunicationSubsystem(State, factory, minPort, maxPort);
        }

        virtual protected void SetStatus(ProcessInfo.StatusCode status) {
            if( StatusIsPossible(status) ) State.ProcessInfo.Status = status;
        }

        public bool StatusIsPossible( ProcessInfo.StatusCode status ) {
            var isPlayer        = State.ProcessInfo.Type == ProcessInfo.ProcessType.Player;
            var isGM = State.ProcessInfo.Type == ProcessInfo.ProcessType.GameManager;
            switch ( status ) {
                case ProcessInfo.StatusCode.HostingGame:     return isGM;
                case ProcessInfo.StatusCode.Won:             return isPlayer;
                case ProcessInfo.StatusCode.Lost:            return isPlayer; 
                case ProcessInfo.StatusCode.Tied:            return isPlayer;
                default: return true;
            }
        }
        public abstract void Logout(Action<bool> callback);
    }
}
