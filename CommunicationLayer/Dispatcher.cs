using log4net;
using MyUtilities;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CommunicationLayer
{
    public class Dispatcher : ExtendedBackgroundThread {

        private static readonly ILog log = LogManager.GetLogger(typeof(Dispatcher));

        public CommunicationSubsystem SubSystem { get; set; }

        public Dispatcher( CommunicationSubsystem subsystem) {
            SubSystem = subsystem;
        }

        override protected void Process(object state) {
            while ( KeepGoing ) {
                Envelope received = SubSystem.PostMan.Receive(1000);
                if ( received != null )  DispatchMessage( received );
            }
        }

        public void DispatchMessage( Envelope env ) {
            ThreadUtil.Run(()=> {
                log.Info( "New Message Received: ("+env.Message.ToString()+")");
                log.Info( "MessageId: " + env.Message.MsgId + "ConversationId: " + env.Message.ConvId);
                NewMessage(env);
            });
        }

        public bool DispatchConversation<T>(Action<T> PropertyInjector=null) where T : class, IConversation {
            IConversation conv = null;
            return DispatchConversation(out conv, PropertyInjector);
        }

        public bool DispatchConversation<T>(out IConversation conv, Action<T> PropertyInjector=null) where T : class, IConversation {
            conv = SubSystem.Factory.Create<T>();
            if(conv != null && PropertyInjector != null) PropertyInjector(conv as T);
            if( NewConversation(conv, true)) {
                while(conv.HasCompleted == false) {
                    Thread.Sleep(100);
                }
                return conv.Success;
            }
            else return false;
        }


        async public Task<bool> DispatchConversationAsync<T>(Action<T> PropertyInjector=null) where T : class, IConversation {
            return await Task.Run(()=> {
                return DispatchConversation(PropertyInjector);
            });
        }
        async public Task DispatchConversationAsync<T>(Action<bool> callback, Action<T> PropertyInjector=null) where T : class, IConversation {
            var success = await DispatchConversationAsync(PropertyInjector);
            callback(success);
        }

        public void NewMessage( Envelope env ) {
            if( AddEnvelopeToQueue( env ) == false) {
                log.Debug("Conversation " + " with id= "+env.Message.ConvId + " does not exist, trying to create new conversation.");
                var conv = SubSystem.Factory.Create( env );
                if( NewConversation( conv, false ) && AddEnvelopeToQueue( env ) ) {
                    conv.Execute();
                }
                else log.Error("An error was encountered while creating a new conversation queue");
            }
        }

        public bool AddEnvelopeToQueue( Envelope env ) {
            if (SubSystem.QueueLookup.QueueExists(env)) {
                SubSystem.QueueLookup[env].Enqueue(env);
                log.Debug("Successsfully queued a new message of type "+ env.Message.GetType().Name +", for conversation: " + env.Message.ConvId);
                return true;
            }
            return false;
        }

        public bool NewConversation( IConversation conv, bool doExecute = true ) {
            if( conv == null ) {
                log.Error("New conversation returned from factory is Null!");
                return false;
            }
            log.Debug("Adding Conversation " + conv.ToString() + ", "+conv.ConversationId);
            var newQueue = SubSystem.QueueLookup.AddQueue( conv );
            log.Debug("      New Queue ID: " + newQueue.QueueId );
            if(doExecute) {
                conv.Execute();
            }
            return true;
        }
    }
}
