using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedObjects;

namespace CommunicationLayer
{
    public class ConversationQueue {

        protected ConcurrentQueue<Envelope> _queue;

        public MessageNumber QueueId {
            get { return Conversation.ConversationId; }
        }
        public int           Count   { get { return _queue.Count; } }
        public IConversation Conversation { get; set; }

        public ConversationQueue (IConversation conversation) {
            _queue = new ConcurrentQueue<Envelope>();
            Conversation = conversation;
            Conversation.ConversationQueue = this;
        }

        public void Enqueue ( Envelope envelope ) {
            _queue.Enqueue( envelope );
        }

        public Envelope Dequeue () {
            Envelope envelope = null;
            bool success = false;
            while(success == false && Count > 0) {
                success = _queue.TryDequeue( out envelope );
            }
            return envelope;
        }

        public Envelope Peek() {
            Envelope envelope = null;
            bool success = false;
            while(success == false && Count > 0) {
                success = _queue.TryPeek( out envelope );
            }
            return envelope;
        }
    }
}
