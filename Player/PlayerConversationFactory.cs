using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using CommunicationLayer;
using Player.Conversation;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using Messages;
namespace Player
{
   public class PlayerConversationFactory : ConversationFactory {

        public PlayerConversationFactory() : base() {
            Add( new Dictionary<Type, Type>{
                   // Initiated
                 { typeof( GameListConversation     ), typeof( GameListConversation          ) },
                 { typeof( JoinGameConversation     ), typeof( JoinGameConversation          ) },
                 { typeof( LoginConversation        ), typeof( LoginConversation             ) },
                 { typeof( LogoutConversation       ), typeof( LogoutConversation            ) },
                 { typeof( BuyBalloonConversation   ), typeof( BuyBalloonConversation        ) },
                 { typeof( FillBalloonConversation  ), typeof( FillBalloonConversation       ) },
                 { typeof( ThrowBalloonConversation ), typeof( ThrowBalloonConversation      ) },
                 { typeof( LeaveGameConverastion    ), typeof( LeaveGameConverastion         ) },

                 // Received
                 { typeof( AliveRequest             ), typeof( AliveConversation             ) },
                 { typeof( GameStatusNotification   ), typeof( GameStatusConversation        ) },
                 { typeof( AllowanceDeliveryRequest ), typeof( AllowanceDeliveryConversation ) },
                 { typeof( ReadyToStart             ), typeof( GameStartConversation         ) },
                 { typeof( HitNotification          ), typeof( HitByBalloonConversation      ) },
                 { typeof( ExitGameRequest          ), typeof( ExitGameConversation          ) },
                 { typeof( ShutdownRequest          ), typeof( ShutdownConversation          ) }
            });
        }
    }
}
