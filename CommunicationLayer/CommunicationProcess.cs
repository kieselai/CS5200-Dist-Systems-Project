using SharedObjects;
using System;
using MyUtilities;

namespace CommunicationLayer
{
    public abstract class CommunicationProcess : ExtendedBackgroundThread {
        public CommunicationSubsystem  SubSystem  { get; set; }
        public ProcessState            State      { get; set; }

        public CommunicationProcess(ProcessState _state, ConversationFactory factory, int minPort, int maxPort) : base() {
            State = _state;
            State.SetStatus(ProcessInfo.StatusCode.NotInitialized);
            SubSystem = new CommunicationSubsystem(State, factory, minPort, maxPort);
        }
        public abstract void Logout(Action<bool> callback);
    }
}