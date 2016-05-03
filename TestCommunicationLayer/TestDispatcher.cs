using System;
using CommunicationLayer;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayerProcess;

namespace TestCommunicationLayer
{
    [TestClass]
    public class TestDispatcher {
        //PostMan RegistryPostman { get; set; }
        //PostMan ProxyPostman    { get; set; }
        //PublicEndPoint ProcessEndpoint  { get; set; }
        //PublicEndPoint RegistryEndpoint { get; set; }
        //PublicEndPoint ProxyEndpoint    { get; set; }
        //PlayerProcess process;

        //public TestDispatcher() {
        //    RegistryPostman = new PostMan();
        //    ProxyPostman    = new PostMan();
        //    process = new PlayerProcess();
        //    process.initializeSubsystem("127.0.0.1:12000", "127.0.0.1:12001");
        //    process.initializePlayer("first", "last", "alias", "anumber");
        //    process.Start();
        //    ProcessEndpoint = new PublicEndPoint("127.0.0.1:"+ process.SubSystem.PostMan.Port);
        //}

        //public MessageNumber NextNum(int procNum) {
        //    int seq;
        //    if(procNum == 1) { testPostmanSeq++; seq = testPostmanSeq; }
        //    else             { ProcessSeq++;     seq = ProcessSeq;     }
        //    return new MessageNumber { Pid = procNum, Seq = seq  };
        //}

        //public Envelope SendToProcess(Message m, bool isInitiated=false) {
        //    var msgNumber = NextNum(1);
        //    if( isInitiated ) m.ConvId = msgNumber;
        //    m.MsgId = msgNumber;
        //    return new Envelope(m, ProcessEndpoint);
        //}

        //[TestMethod]
        //public void TestAliveRequest() {
        //    SendToProcess( new AliveRequest(), true);
        //    var msg = TestPostman.Receive(1000);
        //    Assert.IsTrue( result1 );
        //    Assert.AreEqual( ((Reply)result2.Message).Note, msg1.Note);         
        //}


        //[TestMethod]
        //public void TestAliveReply() {
        //    process.Start();
        //    var id = MessageNumber.Create();
        //    var msg = new AliveRequest { ConvId = id, MsgId = id };
        //    var env = new Envelope(msg, new PublicEndPoint("127.0.0.1:" + process.SubSystem.PostMan.Port));
        //    var result1 = p1.Send(env, 1000);
        //    var result2 = p1.Receive(1000);
        //    process.Stop();
        //    Assert.IsTrue(result1);
        //    Assert.AreEqual(result2.Message.ConvId, id);
        //}
    }
}
