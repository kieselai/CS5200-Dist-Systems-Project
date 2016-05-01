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
    abstract public class InitiatedConversation : ReceivingConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(InitiatedConversation));
        abstract protected void     ProcessFailure();
        abstract protected Envelope CreateRequest();

        protected override bool ConversationStart() {
            var success = true;
            var env = CreateRequest();
            if( env == null ) success = false;
            else {
                log.Debug(GetLabel + "Conversation Id  for initiated conversation: "+ ConversationId);
                if( SendEnvelope( env, true ) == false ) {
                    log.Error(GetLabel + "Unable to send message. ");
                    success = false;
                }
                else success = true;
            }
            if(success) {
                EventQueue.Enqueue( ()=> {
                    if(MessageIsAvailable()) {
                        var receivedEnv = RetrieveNext();
                        if(receivedEnv == null) return false;
                        return ProcessIncoming(receivedEnv);
                    }
                    return false;
                });
            }
            return success;            
        }
    }
}