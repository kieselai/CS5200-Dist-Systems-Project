using System.Net;
using SharedObjects;
using Messages;


namespace CommunicationLayer
{
    public class Envelope {
        public PublicEndPoint Destination { get; set; }
        public Message        Message     { get; set; }

        public Envelope ( Message message, PublicEndPoint endpoint ) {
            Message     = message;
            Destination = endpoint;
        }
    }
    // Allows us to write " env.Unwrap " without encountering null reference exceptions for null envelopes
    public static class EnvelopeExtensions {

        public static T Unwrap<T>(this Envelope env) where T : Message {
            if ( env == null ) return null;
            else if( env.Message.GetType() == typeof(Routing) && typeof(T) != typeof(Routing) )
                return (env.Message as Routing).InnerMessage as T;
            else return env.Message as T;
        }

        public static Envelope SetMessageIds(this Envelope env, MessageNumber ConversationId, bool isInitiated) {
            if(env == null) return null;
            env.Message.ConvId = ConversationId;
            env.Message.MsgId = isInitiated? ConversationId : MessageNumber.Create();
            if (env.Message.GetType() == typeof(Routing)) {
                ((Routing)env.Message).InnerMessage.ConvId = ConversationId;
                ((Routing)env.Message).InnerMessage.MsgId  = env.Message.MsgId;
            }
            return env;
        }
    }
}
