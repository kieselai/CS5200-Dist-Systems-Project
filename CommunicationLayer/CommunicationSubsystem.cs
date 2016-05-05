

namespace CommunicationLayer
{
    public class CommunicationSubsystem {
        // Generic implementations that will not be overridden ( Uses private setter )
        public PostMan                   PostMan           { get; private set; }
        public ConversationQueueLookup   QueueLookup       { get; private set; }
        public Dispatcher                Dispatcher        { get; private set; }
        public AddressManager            AddressManager    { get; private set; }

        // Abstract classes that are overriden to Specialized types in subClasses
        public ConversationFactory     Factory           { get; protected set; }
        public ProcessState            State             { get; protected set; }

        public CommunicationSubsystem(ProcessState state, ConversationFactory factory, int minPort, int maxPort) {
            State = state;
            QueueLookup = ConversationQueueLookup.Instance;
            AddressManager = new AddressManager();
            Factory = factory;
            Factory.SubSystem = this;
            PostMan = new PostMan(minPort, maxPort);
            Dispatcher = new Dispatcher(this);
        }
    }
}
