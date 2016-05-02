using System;
using System.Collections.Generic;
using CommunicationLayer;
using ProcessCommon.Conversation;
using Messages.RequestMessages;
namespace ProcessCommon
{
   public class CommonConversationFactory : ConversationFactory {

        public CommonConversationFactory() : base() {
            Add( new Dictionary<Type, Type>{
                   // Initiated
                 { typeof( LogoutConversation             ), typeof( LogoutConversation            ) },
                 { typeof( LeaveGameConverastion          ), typeof( LeaveGameConverastion         ) },
                 // Received
                 { typeof( GameStatusNotification         ), typeof( GameStatusConversation        ) },
                 { typeof( ExitGameRequest                ), typeof( ExitGameConversation          ) },
                 { typeof( ShutdownRequest                ), typeof( ShutdownConversation          ) }
            });
        }
    }
}
