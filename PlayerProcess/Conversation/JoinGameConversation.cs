﻿using SharedObjects;
using Messages.ReplyMessages;
using ProcessCommon.Conversation;
using Utils;
using CommunicationLayer;

namespace PlayerProcess.Conversation
{
    public class JoinGameConversation : AbstractJoinGameConversation {
        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }
        override protected bool SetAndVerifyIds() {
            if ( PlayerState.OpenGames != null && PlayerState.OpenGames.Count > 0 ) {
                var openGames = PlayerState.OpenGames;
                GameInfo nextAvailableGame = null;
                SyncUtils.WaitForCondition( ()=> openGames.TryDequeue( out nextAvailableGame ) || openGames.Count == 0, 10, 10);
                if ( nextAvailableGame != null ) {
                    GameId = nextAvailableGame.GameId;
                    GameManagerId = nextAvailableGame.GameManagerId;
                    return true;
                }
            }
            return MessageFailure("No games available");
        }

        protected override bool ProcessReply() {
            if ( base.ProcessReply() == true ) {
                var reply = IncomingMessage.Unwrap<JoinGameReply>();
                if ( reply.InitialLifePoints <= 0 ) {
                    return MessageFailure("Initial life points is less than or equal to zero.");
                }
                else {
                    PlayerState.InitialLifePoints = reply.InitialLifePoints;
                    SubSystem.State.ProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
                    return MessageSuccess();
                }
            }
            else return false;
        }
    }
}
