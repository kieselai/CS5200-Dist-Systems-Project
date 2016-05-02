using SharedObjects;
using System;
using System.Collections.Generic;
using Utils;


namespace CommunicationLayer
{
    public interface ICommunicationProcess {
        CommunicationSubsystem  SubSystem { get; set; }
        ProcessState            State     { get; set; }
        void Start(object state);
        void Stop();
        void Logout(Action<bool> callback);
    }

    public abstract class CommunicationProcess<Conversation_Factory, Process_State> : BackgroundThread, ICommunicationProcess 
            where Conversation_Factory : ConversationFactory, new() where Process_State : ProcessState, new() {
        public CommunicationSubsystem  SubSystem  { get; set; }
        public ProcessState            State      { get; set; }
        public Process_State           TypedState { get { return State as Process_State; } }

        public CommunicationProcess() : base() {
            State = new Process_State();
            SetStatus(ProcessInfo.StatusCode.NotInitialized);
        }

        public void initializeSubsystem() {
            SubSystem = new CommunicationSubsystem(new Conversation_Factory(), State);
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
