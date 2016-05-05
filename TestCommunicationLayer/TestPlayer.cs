using CommunicationLayer;
using SharedObjects;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayerProcess;
using System;
using System.Threading;
using System.Diagnostics;
using Messages;
using Utils;
using System.Collections.Generic;

namespace TestCommunicationLayer
{
    [TestClass]
    public class TestPlayer : DummyGame {

        public TestPlayer() : base() {}

        [TestMethod]
        public void TestGameListRequest() {
            var index = init();
            TestGameListRequest(index);
        }

        public void TestGameListRequest(int index) {
            player.StartProcess(index, this, ProcessInfo.StatusCode.Registered);
            Envelope result = registry.ReceiveMessageOfType<GameListRequest>(index, "Registry");
            Assert.IsTrue( ValidateMessage<GameListRequest>( result ) );
            var gameListRequest = result.Message as GameListRequest;
            Assert.AreEqual(gameListRequest.StatusFilter, 4);
            Assert.AreEqual(gameListRequest.ConvId.Pid, 1);
            ConversationId(index, gameListRequest.ConvId);
        }

        public GameInfo DummyGameInfo {
            get {
                return new GameInfo {
                    GameId = 77,
                    GameManagerId = 1,
                    Label = "TestGame",
                    MaxPlayers = 3,
                    MinPlayers = 3,
                    StartingPlayers = new int[] { 1, 3 },
                    Status = GameInfo.StatusCode.Available,
                    Winners = null,
                    CurrentProcesses = new GameProcessData[] {
                             new GameProcessData {
                                 HasUmbrellaRaised = false,
                                 HitPoints=10,
                                 LifePoints = 12,
                                 ProcessId=1000,
                                 Type = ProcessInfo.ProcessType.Player
                             }
                        }
                };
           }
        }

        [TestMethod]
        public void TestGameListReply() {
            var index = init();
            TestGameListReply(index);
        }

        public void TestGameListReply(int index) {
            TestGameListRequest(index);
            var env = AddressManager.AddressTo( new GameListReply {
                Note ="Test",
                Success =true,
                GameInfo = new GameInfo[] { DummyGameInfo }
            }, player.getEp(index));
            env.SetMessageIds(ConversationId(index), false);
            registry.SendWithRetries(index, "Registry", env, ()=> (player[index].State as PlayerState).OpenGames != null);
            Assert.IsNotNull( (player[index].State as PlayerState).OpenGames);
            Assert.AreEqual(  (player[index].State as PlayerState).OpenGames.Count, 1);
            GameInfo openGame=null;
            SyncUtils.WaitForCondition(()=> (player[index].State as PlayerState).OpenGames.TryDequeue(out openGame));
            Assert.IsNotNull(openGame);
            Assert.AreEqual(openGame.GameId, 77);
        }

        [TestMethod]
        public void TestGameStart() {
            var index = init();
            player[index].State.CurrentGame = DummyGameInfo;
            player.StartProcess(index, this, ProcessInfo.StatusCode.JoinedGame);
            var env = AddressManager.AddressTo(new ReadyToStart {
                GameId = 8
            }, player.getEp(index));
            env.SetMessageIds(ConversationId(index), true);
            var result = proxy.SendReceiveWithRetries<Routing>(index, "Proxy",  env);
            SyncUtils.WaitForCondition( ()=>  (player[index].State as PlayerState).OpenGames != null, 5000, 100 );
            Assert.IsTrue( ValidateMessage<StartGame>( result ) );
            var gameStartReply = (result.Message as Routing).InnerMessage as StartGame;
            Assert.AreEqual(gameStartReply.Success, true);
        }
    }
}
