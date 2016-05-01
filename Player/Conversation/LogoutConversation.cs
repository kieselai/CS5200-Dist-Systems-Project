﻿using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System;

namespace Player.Conversation
{
    public class LogoutConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogoutConversation));
        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }
        override protected bool CreateRequest(){
            log.Debug("Queuing logout request. ");
            OutgoingMessage = AddressTo( new LogoutRequest(),"Registry");
            return true;
        }
        override protected bool ProcessReply(){
            var logoutReply = Cast<Reply>(IncomingMessage);
            if ( logoutReply.Success ) {
                PlayerState.LoggedOut = true;
                Success = true;
                return true;
            }
            else MessageFailure("Logout request failed");
            return false;
        }
    }
}