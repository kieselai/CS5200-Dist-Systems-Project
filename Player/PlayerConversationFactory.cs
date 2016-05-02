using System;
using System.Collections.Generic;
using Player.Conversation;
using Messages.RequestMessages;
using ProcessCommon;
namespace Player
{
    public class PlayerConversationFactory : CommonConversationFactory {

        public PlayerConversationFactory() : base() {
            Add( new Dictionary<Type, Type>{
                 // Initiated
                 { typeof( GameListConversation     ), typeof( GameListConversation          ) },
                 { typeof( JoinGameConversation     ), typeof( JoinGameConversation          ) },
                 { typeof( LoginConversation        ), typeof( LoginConversation             ) },
                 { typeof( BuyBalloonConversation   ), typeof( BuyBalloonConversation        ) },
                 { typeof( FillBalloonConversation  ), typeof( FillBalloonConversation       ) },
                 { typeof( ThrowBalloonConversation ), typeof( ThrowBalloonConversation      ) },
                 // Common initiated: LogoutConversation, LeaveGameConversation

                 // Received
                 { typeof( AliveRequest             ), typeof( AliveConversation             ) },
                 { typeof( AllowanceDeliveryRequest ), typeof( AllowanceDeliveryConversation ) },
                 { typeof( ReadyToStart             ), typeof( GameStartConversation         ) },
                 { typeof( HitNotification          ), typeof( HitByBalloonConversation      ) }
                 // Common Received: GameStatusNotification, ExitGameRequest, ShutdownRequest
            });
        }
    }
}
