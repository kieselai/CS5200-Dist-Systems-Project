using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using CommunicationLayer;
using BalloonStore.Conversation;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using Messages;
namespace BalloonStore
{
   public class BalloonStoreConversationFactory : ConversationFactory {

        public BalloonStoreConversationFactory() : base() {
            Add( new Dictionary<Type, Type>{
                   // Initiated
                 { typeof( JoinGameConversation     ), typeof( JoinGameConversation          ) },
                 { typeof( LoginConversation        ), typeof( LoginConversation             ) },
                 { typeof( LogoutConversation       ), typeof( LogoutConversation            ) },
                 { typeof( LeaveGameConverastion    ), typeof( LeaveGameConverastion         ) },

                 // Received
                 { typeof( BuyBalloonRequest        ), typeof( BuyBalloonConversation        ) },
                 { typeof( GameStatusNotification   ), typeof( GameStatusConversation        ) },
                 { typeof( ReadyToStart             ), typeof( GameStartConversation         ) },
                 { typeof( ShutdownRequest          ), typeof( ShutdownConversation          ) }
            });
        }
    }
}
