﻿using CommunicationLayer;
using System.Collections.Concurrent;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System;
using Messages;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Player.Conversation
{
     public class HitByBalloonConversation : ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HitByBalloonConversation));

        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }
        
        override protected bool ProcessRequest() {
            var hitNotification = Cast<HitNotification>( IncomingMessage );
            if(IncomingMessage == null) {
                MessageFailure("Hit Notification was null");
                return false;
            }
            else if (hitNotification == null) {
                MessageFailure("Failed to cast Hit Notification");
                return false;
            }
            else {
                PlayerState.HitPoints += 1;
                Success = true;
                return true;
            }
        }

        protected override bool CreateResponse() {
            OutgoingMessage = RouteTo(new Reply {
                Note = "Received",
                Success = true
            }, PlayerState.CurrentGame.GameManagerId );
            return true;
        }
    }
}