using SharedObjects;
using System;
using Utils;


namespace CommunicationLayer
{
    public abstract class CommunicationProcess : BackgroundThread {
        public CommunicationSubsystem      SubSystem { get; set; }
        public    ProcessState             State     { get; set; }

        public CommunicationProcess(ProcessState processState) : base() {
            State = processState;
        }

        public void initializeSubsystem(ConversationFactory factory, EndpointLookup endpointLookup) {
            SubSystem = new CommunicationSubsystem(factory, endpointLookup, State);
        }

        protected override void Process(object state) {
            throw new NotImplementedException();
        }

        virtual protected void SetStatus(ProcessInfo.StatusCode status) {
            if( StatusIsPossible(status)) { }
            State.ProcessInfo.Status = status;
        }

        public bool StatusIsPossible( ProcessInfo.StatusCode status ) {
            var isGameManager   = State.ProcessInfo.Type == ProcessInfo.ProcessType.GameManager;
            var isPlayer        = State.ProcessInfo.Type == ProcessInfo.ProcessType.Player;
            switch ( status ) {
                case ProcessInfo.StatusCode.PlayingGame:     return isPlayer;
                case ProcessInfo.StatusCode.HostingGame:     return isGameManager;
                case ProcessInfo.StatusCode.LeavingGame:     return isPlayer;
                case ProcessInfo.StatusCode.Won:             return isPlayer;
                case ProcessInfo.StatusCode.Lost:            return isPlayer; 
                case ProcessInfo.StatusCode.Tied:            return isPlayer;
                default: return true;
            }
        }
    }
}
