using System.Collections.Concurrent;
using SharedObjects;
using Messages;

namespace CommunicationLayer
{
    public class ConversationQueueLookup {

#region Data Members and constructor

        private static ConversationQueueLookup instance;

        protected ConcurrentDictionary<MessageNumber, ConversationQueue> _QueueLookup;

        private ConversationQueueLookup () {
            _QueueLookup = new ConcurrentDictionary<MessageNumber, ConversationQueue>( );
        }

#endregion

#region Singleton accessor
        public static ConversationQueueLookup Instance {
            get {
                if ( instance == null )
                    instance = new ConversationQueueLookup();
                return instance;
           }
        }
        #endregion

#region Accessors
        public ConversationQueue this[ MessageNumber id ] {
            get { return getQueueById( id );  }
        }
        public ConversationQueue this[ IConversation conv ] {
            get { return this[ conv.ConversationId ];  }
        }
        public ConversationQueue this[ Envelope env ] {
            get { return getQueueById( env.Message.ConvId );  }
        }
        public ConversationQueue this[ Message msg ] {
            get { return this[ msg.ConvId ];  }
        }
#endregion

#region Queue Exists Checks
        public bool QueueExists(IConversation conv) {
            return QueueExists(conv.ConversationId);
        }
        public bool QueueExists(Envelope env) {
            return QueueExists(env.Message.ConvId);
        }
        public bool QueueExists(Message msg) {
            return QueueExists(msg.ConvId);
        }
        public bool QueueExists(MessageNumber id) {
            return _QueueLookup.ContainsKey(id);
        }
#endregion

         public ConversationQueue getQueueById( MessageNumber id ) {
            if (QueueExists( id )) return _QueueLookup[ id ];
            else return null;
        }

        public ConversationQueue AddQueue( IConversation conv ) {
            if ( QueueExists(conv) == false )
                _QueueLookup.AddOrUpdate( conv.ConversationId, new ConversationQueue(conv), (k, v) => v );
            return this[ conv ];
        }

        public ConversationQueue DeleteQueue ( MessageNumber QueueId ) {
            ConversationQueue removed;
            var result = _QueueLookup.TryRemove( QueueId, out removed );
            if ( result )  return removed;
            else return null;
        }
    }
}
