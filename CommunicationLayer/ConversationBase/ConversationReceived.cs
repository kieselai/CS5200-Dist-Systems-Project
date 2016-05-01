using SharedObjects;
using Messages;
using Utils;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Collections.Concurrent;

namespace CommunicationLayer
{
    abstract public class ReceivingConversation : Conversation {
        protected override bool ConversationStart() {
            var request = RetrieveNext();
            if( !IsValid(request)) return false;
            else return ProcessIncoming(request);
        }

        protected bool CheckConversationComplete(Envelope request) {
            EventQueue.Enqueue( ()=> {
                if(MessageIsAvailable()) return ConversationLoop( ()=> ConversationStart(), 2000);
                HasCompleted = true;
                return true;
            });
            return true;
        }
    }

    abstract public class ResponseConversation : ReceivingConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ResponseConversation));
        abstract protected Envelope CreateResponse(Envelope env);

        
        protected bool Respond(Envelope env) {
            log.Debug(GetLabel + "Conversation Id  for Response conversation: "+ ConversationId);
            if( SendEnvelope( env, false ) == false ) {
                return false;
            }
            return true;
        }
        protected override bool ProcessIncoming(Envelope request) {
            var env = CreateResponse(request);
            if(env != null && Respond(env) == true) {
                return CheckConversationComplete(request);
            }
            return false;
        }
    }

    abstract public class ReceivedMulticast : Conversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ReceivedMulticast));
        protected override bool ConversationStart() {
            if(MessageIsAvailable()) {
                var result = RetrieveNext();
                if(result != null) {
                    EventQueue.Enqueue( ()=> ConversationLoop( ()=> ProcessIncoming(result)));
                    return true;
                }
            }
            return false;
        }

        protected void Restart() {
            EventQueue.Enqueue( ()=> {
                if(ConversationLoop( ()=> ConversationStart()) != true) {
                    HasFailed = true;
                    return false;
                }
                else return true;
            });
        }
    }

    abstract public class RequestReplyWithTCP : ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RequestReplyWithTCP));
        abstract protected bool OpenTCPStream();
        protected void Restart() {
            EventQueue.Enqueue(()=>ConversationLoop( ()=> ConversationStart() && OpenTCPStream()  );
        }
    }
}