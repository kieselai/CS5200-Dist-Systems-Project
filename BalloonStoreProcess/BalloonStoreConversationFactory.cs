using System;
using System.Collections.Generic;
using BalloonStoreProcess.Conversation;
using Messages.RequestMessages;
using ProcessCommon;

namespace BalloonStoreProcess
{
    public class BalloonStoreConversationFactory : CommonConversationFactory {

        public BalloonStoreConversationFactory() : base() {
            Add( new Dictionary<Type, Type>{
                 // Initiated
                 { typeof( JoinGameConversation     ), typeof( JoinGameConversation          ) },
                 { typeof( LoginConversation        ), typeof( LoginConversation             ) },
                 // Common initiated: LogoutConversation, LeaveGameConversation

                 // Received
                 { typeof( BuyBalloonRequest        ), typeof( BuyBalloonConversation        ) },
                 { typeof( ReadyToStart             ), typeof( GameStartConversation         ) }
                 // Common Received: GameStatusConversation, ExitGameConversation, ShutdownConversation
            });
        }
    }
}