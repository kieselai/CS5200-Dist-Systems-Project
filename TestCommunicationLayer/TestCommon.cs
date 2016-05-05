using CommunicationLayer;
using SharedObjects;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Messages;
using Utils;

namespace TestCommunicationLayer
{
    [TestClass]
    public class TestCommon : DummyGame {

        public TestCommon() : base() {}
               
        [TestMethod]
        public void TestSimpleReceive() {
            var postman1 = new PostMan(12000, 20000);
            var postman2 = new PostMan(12000, 20000);
            var id = MessageNumber.Create();
            var msg = new Reply { Note = "Test1: postman1 to postman2", ConvId = id, MsgId = id };
            var env = new Envelope( msg, postman2.LocalEndPoint );
            var result =  TestExtensions.SendReceiveWithRetries<Reply>(env, postman1, postman2);
            Assert.IsTrue( ValidateMessage<Reply>( result ) );
            Assert.AreEqual( ((Reply)result.Message).Note, msg.Note);
            Assert.AreEqual( result.Message.ConvId, id);
            Assert.AreEqual( result.Message.MsgId,  id);
        }

        [TestMethod]
        public void TestAliveReply() {
            var index = init();
            common.StartProcess(index, this, ProcessInfo.StatusCode.JoinedGame);
            var convId = MessageNumber.Create();
            var msg = new AliveRequest { ConvId = convId, MsgId = convId };
            var env = new Envelope(msg, common.getEp(index));
            var result = registry.SendReceiveWithRetries<Reply>(index, "Registry", env);
            Assert.IsTrue( ValidateMessage<Reply>( result ) );
            Assert.AreEqual(result.Message.ConvId, convId);
            common[index].Stop();
        }

        [TestMethod]
        public void TestLogin() {
            var index = init();
            TestLogin(index);
        }

        public void TestLogin(int index) {
            common.StartProcess(index, this, ProcessInfo.StatusCode.NotInitialized);
            var result = registry.ReceiveMessageOfType<LoginRequest>(index, "Registry");
            Assert.IsTrue( ValidateMessage<LoginRequest>( result ) );
            ConversationId(index, result.Message.ConvId);
            Trace.WriteLine(ConversationId(index).Pid + ":"+ConversationId(index).Seq);
        }        

        [TestMethod]
        public void TestLoginReply() {
            var index = init();
            TestLogin(index);
            TestLoginReply(index);
        }

        public void TestLoginReply(int index) {
            TestLogin(index);
            Assert.IsNotNull(ConversationId(index));
            Assert.IsNotNull(proxy[index].LocalEndPoint);
            var env = new Envelope(
                new LoginReply {
                    ConvId = ConversationId(index),
                    MsgId  = MessageNumber.Create(),
                    PennyBankEndPoint = pennyB[index].LocalEndPoint,
                    ProcessInfo = common.DummyProcessInfo(index),
                    ProxyEndPoint = proxy[index].LocalEndPoint,
                    Success = true
                }, 
                common.getEp(index)
            );
            var success = registry.SendWithRetries(index, "Registry", env, ()=> {
                return common.epLookup(index, "Proxy") != null && common.epLookup(index, "PennyBank") != null;
            });
            Assert.AreEqual(  common[index].Label, common.DummyProcessInfo(index).Label);
            Assert.IsTrue( success );
            Assert.AreEqual( common.epLookup(index, "Proxy").Port,     proxy[index].LocalEndPoint.Port);
            Assert.AreEqual( common.epLookup(index, "PennyBank").Port, pennyB[index].LocalEndPoint.Port);
        }

        [TestMethod]
        public void TestGameStart() {
            var index = init();
            common[index].State.CurrentGame = new GameInfo {
                GameManagerId = 10
            };
            common.StartProcess(index, this, ProcessInfo.StatusCode.Registered);
            var num = MessageNumber.Create();
            var env = new Envelope(new ReadyToStart {
                ConvId = num,
                MsgId  = num,
                GameId = 8
            }, common.getEp(index));
            var result = proxy.SendReceiveWithRetries<Routing>(index, "Proxy", env);
            SyncUtils.WaitForCondition( ()=> common[index].State.CurrentGame != null, 5000, 100 );
            Assert.IsTrue( ValidateMessage<StartGame>( result ) );
            var gameStartReply = (result.Message as Routing).InnerMessage as StartGame;
            Assert.AreEqual(gameStartReply.Success, true);
        }

    }
}
