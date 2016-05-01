﻿using System.Collections.Generic;
using Messages;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using SharedObjects;
using log4net;
using Utils;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace CommunicationLayer
{
    public class Dispatcher : BackgroundThread {

        private static readonly ILog log = LogManager.GetLogger(typeof(Dispatcher));

        public CommunicationSubsystem SubSystem { get; set; }

        public Dispatcher () {}

        override protected void Process(object state) {
            while ( KeepGoing ) {
                Envelope received = SubSystem.PostMan.Receive(1000);
                if ( received != null )  DispatchMessage( received );
            }
        }

        async public void DispatchMessage( Envelope env ) {
            await Task.Run(()=> {
                log.Info( "New Message Received: ("+env.Message.ToString()+")");
                log.Info( "MessageId: " + env.Message.MsgId + "ConversationId: " + env.Message.ConvId);
                NewMessage(env);
            });
        }

        public bool DispatchConversation<T>() where T : IConversation {
            IConversation conv = SubSystem.Factory.Create<T>();
            if( NewConversation(conv, true)) {
                while(conv.HasCompleted == false) {
                    Thread.Sleep(100);
                }
                return conv.Success;
            }
            else return false;
        }

        async public Task<bool> DispatchConversationAsync<T>() where T : IConversation {
            return await Task.Run(()=> {
                return DispatchConversation<T>();
            });
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