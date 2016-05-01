using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLayer
{
    static public class MessageValidator {
        public static bool IsValid(this Envelope env) {
            return env != null && env.Destination != null && env.Message.IsValid();
        }

        public static bool IsValid<T>(this Envelope env) {
            return env.IsValid() && env.Message.IsValid<T>();
        }

        public static bool IsValid(this Message m) {
            var isValid = m != null && m.HasIDs();
            if(m.GetType() == typeof(Routing)) {
                return isValid && ((Routing)m).InnerMessage.IsValid();
            }
            else return isValid;
        }

        public static bool IsValid<T>(this Message m) {
            var isValid = m.IsValid();
            if(m.GetType() == typeof(Routing)) {
                return isValid && ((Routing)m).InnerMessage.GetType() == typeof(T);
            }
            else return isValid && m.GetType() == typeof(T);
        }

        public static bool HasIDs(this Message m) {
            if(m == null || m.MsgId == null || m.ConvId == null) return false;
            else if( m.GetType() == typeof(Routing)) {
                var innerM = ((Routing)m).InnerMessage;
                return innerM != null && innerM.MsgId != null && innerM.ConvId != null;
            }
            return true;
        }

        public static string InvalidMessage(this Envelope env) {
            if( env == null )                  return "Envelope is null";
            else if( env.Destination == null ) return "Envelope has no destination";
            else if( env.Message == null )     return "Envelope has no message"; 
            return env.Message.InvalidMessage();
        }

        public static string InvalidMessage(this Message m) {
                 if( m.IsValid())       return "Message is valid";
            else if( !m.HasIDs()     )  return "Message does not have a Message Id or a Conversation Id";
            else if( m.MsgId == null )  return "Message does not have a Message Id";
            else                        return "Message does not have a Conversation Id";
        }

        public static string InvalidMessage<T>(this Envelope env) {
            if(env.Message != null && !env.Message.IsValid<T>()) {
                return env.Message.InvalidMessage<T>();
            }
            else return env.InvalidMessage();
        }

        public static string InvalidMessage<T>(this Message m) {
                 if( m.IsValid<T>())    return "Message is valid";
            else if( !m.HasIDs()     )  return "Message does not have a Message Id of a Conversation Id";
            else if( m.MsgId == null )  return "Message does not have a Message Id";
            else if( m.ConvId == null)  return "Message does not have a Conversation Id";
            else return string.Format("Message of type {0} was expected, {1} was received.", typeof(T).Name, m.GetType().Name);
        }
    }
}
