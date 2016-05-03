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
    public class TestPostman : DummyGame {

        public TestPostman() : base() {}
               
        [TestMethod]
        public void TestSimpleReceive() {
            var postman1 = new PostMan(12000, 20000);
            var postman2 = new PostMan(12000, 20000);
            var id = MessageNumber.Create();
            var msg = new Reply { Note = "Test1: postman1 to postman2", ConvId = id, MsgId = id };
            var env = new Envelope( msg, postman2.LocalEndPoint );
            var result =  SendReceiveWithRetries<Reply>(env, postman1, postman2 );
            Assert.IsTrue( ValidateMessage<Reply>( result ) );
            Assert.AreEqual( ((Reply)result.Message).Note, msg.Note);
            Assert.AreEqual( result.Message.ConvId, id);
            Assert.AreEqual( result.Message.MsgId,  id);
        }

        [TestMethod]
        public void TestAliveReply() {
            var index = initPlayer();
            StartProcess(index, ProcessInfo.StatusCode.JoinedGame);
            var convId = MessageNumber.Create();
            var msg = new AliveRequest { ConvId = convId, MsgId = convId };
            var env = new Envelope(msg, PlayerEP(index));
            var result = SendReceiveWithRetries<AliveRequest, Reply>(env, index, "Registry", registry);
            Assert.IsTrue( ValidateMessage<Reply>( result ) );
            Assert.AreEqual(result.Message.ConvId, convId);
            player[index].Stop();
        }

        [TestMethod]
        public void TestLogin() {
            var index = initPlayer();
            TestLogin(index);
        }

        public void TestLogin(int index) {
            StartProcess(index, ProcessInfo.StatusCode.NotInitialized);
            var result = ReceiveMessageOfType<LoginRequest>(index, "Registry", registry);
            Assert.IsTrue( ValidateMessage<LoginRequest>( result ) );
            CurrentConvId[index] = result.Message.ConvId;
            Trace.WriteLine(CurrentConvId[index].Pid + ":"+CurrentConvId[index].Seq);
        }

        

        [TestMethod]
        public void TestLoginReply() {
            var index = initPlayer();
            TestLogin(index);
            TestLoginReply(index);
        }

        public void TestLoginReply(int index) {
            TestLogin(index);
            Assert.IsNotNull(CurrentConvId[index]);
            Assert.IsNotNull(proxy[index].LocalEndPoint);
            var env = new Envelope(
                new LoginReply {
                    ConvId = CurrentConvId[index],
                    MsgId  = MessageNumber.Create(),
                    PennyBankEndPoint = pennyB[index].LocalEndPoint,
                    ProcessInfo = DummyProcessInfo(index),
                    ProxyEndPoint = proxy[index].LocalEndPoint,
                    Success = true
                }, 
                PlayerEP(index)
            );
            var success = SendWithRetries<LoginReply>(env, index, "Registry", registry, ()=> {
                return player[index].SubSystem.EndpointLookup["Proxy"]     != null
                    && player[index].SubSystem.EndpointLookup["PennyBank"] != null;
            });
            Assert.AreEqual(  player[index].Label, DummyProcessInfo(index).Label);
            Assert.IsTrue( success );
            Assert.AreEqual(  player[index].SubSystem.EndpointLookup["Proxy"].Port,     proxy[index].LocalEndPoint.Port);
            Assert.AreEqual(  player[index].SubSystem.EndpointLookup["PennyBank"].Port, pennyB[index].LocalEndPoint.Port);
        }

        [TestMethod]
        public void TestGameListRequest() {
            var index = initPlayer();
            TestGameListRequest(index);
        }

        public void TestGameListRequest(int index) {
            StartProcess(index);
            Envelope result = ReceiveMessageOfType<GameListRequest>(index, "Registry", registry);
            Assert.IsTrue( ValidateMessage<GameListRequest>( result ) );
            var gameListRequest = result.Message as GameListRequest;
            Assert.AreEqual(gameListRequest.StatusFilter, 4);
            Assert.AreEqual(gameListRequest.ConvId.Pid, 1);
            CurrentConvId[index] = gameListRequest.ConvId;
        }

        


        [TestMethod]
        public void TestGameListReply() {
            var index = initPlayer();
            TestGameListReply(index);
        }

        public void TestGameListReply(int index) {
            TestGameListRequest(index);
            var id = MessageNumber.Create();
            var msg = new GameListReply {
                ConvId = CurrentConvId[index],
                MsgId = id,
                Note ="Test",
                Success =true,
                GameInfo = new GameInfo[] {
                    new GameInfo {
                        GameId=77, 
                        GameManagerId = 1, 
                        Label = "TestGame", 
                        MaxPlayers = 3, 
                        MinPlayers = 3,
                        StartingPlayers = new int[] {1, 3 }, 
                        Status = GameInfo.StatusCode.Available,
                        Winners = null,
                        CurrentProcesses = new GameProcessData[] {
                             new GameProcessData {
                                 HasUmbrellaRaised = true, 
                                 HitPoints=10, 
                                 LifePoints = 12, 
                                 ProcessId=7,
                                 Type = ProcessInfo.ProcessType.Player
                             }
                        }
                    }
                },  
            };
            var env = new Envelope(msg, PlayerEP(index));
            SendWithRetries<GameListReply>(env, index, "Registry", registry, ()=>  (player[index].State as PlayerState).OpenGames != null);
            Assert.IsNotNull( (player[index].State as PlayerState).OpenGames);
            Assert.AreEqual(  (player[index].State as PlayerState).OpenGames.Count, 1);
            GameInfo openGame=null;
            SyncUtils.WaitForCondition(()=> (player[index].State as PlayerState).OpenGames.TryDequeue(out openGame));
            Assert.IsNotNull(openGame);
            Assert.AreEqual( openGame.GameId, 77);
        }

        [TestMethod]
        public void TestGameStart() {
            var index = initPlayer();
            (player[index].State as PlayerState).CurrentGame = new GameInfo {
                GameManagerId = 10
            };
            StartProcess(index, ProcessInfo.StatusCode.JoinedGame);
            
            var num = MessageNumber.Create();
            var env = new Envelope(new ReadyToStart {
                ConvId = num,
                MsgId  = num,
                GameId = 8
            }, PlayerEP(index));
            var result = SendReceiveWithRetries<ReadyToStart, Routing>(env, index, "Proxy",  proxy);
            SyncUtils.WaitForCondition( ()=>  (player[index].State as PlayerState).OpenGames != null, 5000, 100 );
            Assert.IsTrue( ValidateMessage<StartGame>( result ) );
            var gameStartReply = (result.Message as Routing).InnerMessage as StartGame;
            Assert.AreEqual(gameStartReply.Success, true);
        }

        public Envelope SendReceiveWithRetries<T1, T2>(Envelope env, int i, string procName, List<PostMan> process)  where T1 : Message where T2 : Message {
            PrintSendReceiveToPlayer<T1, T2>(i, procName, process);
            return SendReceiveWithRetries<T2>(env, process[i]);
        }

        public Envelope SendReceiveWithRetries<T>(Envelope env, PostMan sender, PostMan receiver = null) where T : Message {
            if(receiver == null) receiver = sender;
            Envelope result = null;
            int repeats = 0;
            do {
                ThreadPool.QueueUserWorkItem(SendMessage, Tuple.Create(sender, env));
                result = ReceiveMessageOfType<T>(receiver);
                repeats++;
            } while(repeats < 3 && result == null);
            return result;
        }

        public bool SendWithRetries<T>(Envelope env, int i, string procName, List<PostMan> proc, Func<bool> SuccessCondition) {
            PrintSending<T>(i, procName, proc);
            return SendWithRetries(env, proc[i], SuccessCondition);
        }

        public bool SendWithRetries(Envelope env, PostMan sender, Func<bool> SuccessCondition) {
            Envelope result = null;
            int repeats = 0;
            do {
                ThreadPool.QueueUserWorkItem(SendMessage, Tuple.Create(sender, env));
                SyncUtils.WaitForCondition(SuccessCondition, 2500, 50);
                repeats++;
            } while(repeats < 3 && result == null);
            return SuccessCondition();
        }

        public void SendMessage(object state) {
            var tuple = state as Tuple<PostMan, Envelope>;
            Assert.IsNotNull(tuple);
            var postman = tuple.Item1;
            var env = tuple.Item2;
            postman.Send(env, 1000);
        }
        
        public Envelope ReceiveMessageOfType<T>(int i, string procName, List<PostMan> proc) where T : Message {
            PrintReceiving<T>(i, procName, proc);
            return ReceiveMessageOfType<T>(proc[i]);
        }
        

        public Envelope ReceiveMessageOfType<T>(PostMan p) where T : Message{
            Envelope result = null;
            var timeout = new TimeSpan(0,0,10);
            var timer = Stopwatch.StartNew();
            do {
                result = p.Receive(1000);
                if(result != null && result.Message != null && result.Message.GetType() != typeof(T)) {
                    Trace.WriteLine("Message Received, but expected a message of type '" + typeof(T).Name +"' not '" + result.Message.GetType().Name );
                    result = null;
                }
            } while( result == null && timer.Elapsed < timeout);
            if(result == null) Trace.WriteLine("Test method failed while listening for a message of type: " + typeof(T).Name);
            else Trace.WriteLine("Test method listening for a message of type: " + typeof(T).Name);
            return result;
        }
    }
}
