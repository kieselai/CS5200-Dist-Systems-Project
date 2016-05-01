using CommunicationLayer;
using SharedObjects;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Player;
using System;
using System.Threading;
using System.Diagnostics;
using Messages;
using Utils;
using log4net;
using System.Collections.Generic;

namespace TestCommunicationLayer
{
    public class DummyGame {
        private static readonly ILog log = LogManager.GetLogger(typeof(DummyGame));

        public List<PostMan> registry;
        public List<PostMan> pennyB;
        public List<PostMan> proxy;
        public List<PlayerProcess> player;
        public List<MessageNumber> CurrentConvId;
        public DummyGame () {
            registry      = new List<PostMan>();
            pennyB        = new List<PostMan>();
            proxy         = new List<PostMan>();
            player        = new List<PlayerProcess>();
            CurrentConvId = new List<MessageNumber>();
        }

        public LocalEndPoint PlayerEP(int i) {
            return player[i].SubSystem.PostMan.LocalEndPoint;
        }
        public ProcessInfo DummyProcessInfo(int i) {
           return new ProcessInfo {
                AliveReties = 0,
                EndPoint    = PlayerEP(i),
                Label       = player[i].Label,
                ProcessId   = 1,
                Status      = ProcessInfo.StatusCode.Registered,
                Type        = ProcessInfo.ProcessType.Player
            };
        }

        public bool ValidateMessage<T>(Envelope env) {
            log.Debug( env.InvalidMessage<T>() );
            return env.IsValid<T>();
        }

        public int initPlayer() {
            var i = player.Count;
            player  .Add( new PlayerProcess() );
            registry.Add( new PostMan()       );
            pennyB  .Add( new PostMan()       );
            proxy   .Add( new PostMan()       );
            CurrentConvId.Add(new MessageNumber());
            player[i].initializeSubsystem(registry[i].LocalEndPoint.ToString());
            player[i].initializePlayer("first", "last", "alias", "anumber");
            return i;
        }

        public void StartProcess(int i, ProcessInfo.StatusCode currentState = ProcessInfo.StatusCode.Registered) {
            if (    currentState != ProcessInfo.StatusCode.Initializing 
                && currentState != ProcessInfo.StatusCode.NotInitialized) {
                SetDummyPlayerLoggedIn(i);
            }
            player[i].Start();
        }

        public void SetDummyPlayerLoggedIn(int i) {
            player[i].State.ProcessInfo = DummyProcessInfo(i);
            MessageNumber.LocalProcessId = 1;
            player[i].SubSystem.EndpointLookup.Add("Proxy",     proxy [i].LocalEndPoint);
            player[i].SubSystem.EndpointLookup.Add("PennyBank", pennyB[i].LocalEndPoint);
        }

        public void PrintPlayerInfo(int i, string message=null) {
            log.Debug(string.Format("Player[{0}], EP: {1}, Time: {2}", i, PlayerEP(i), DateTime.Now));
            if(!String.IsNullOrEmpty(message)) log.Debug(message);
        }
        public void PrintProcessInfo(int i, string procName, List<PostMan> procArray, string message=null) {
            log.Debug(string.Format("{3}[{0}], EP: {1}, Time: {2}", i, procArray[i].LocalEndPoint, DateTime.Now, procName));
            if(!String.IsNullOrEmpty(message)) log.Debug(message);
        }

        public void PrintSendReceiveToPlayer<T, T2>(int i, string procName, List<PostMan> procArr) {
            PrintSending<T>(i, procName, procArr);
            PrintReceiving<T2>(i);
        }

        public void PrintSendReceiveFromPlayer<T, T2>(int i, string procName, List<PostMan> procArr) {
            PrintSending<T>(i);
            PrintReceiving<T2>(i, procName, procArr);
        }

        public void PrintReceiving<T>(int i) {
            PrintPlayerInfo(i, FormatReceive<T>());
        }
        public void PrintSending<T>(int i) {
            PrintPlayerInfo(i, FormatSend<T>());
        }

        public void PrintReceiving<T>(int i, string procName, List<PostMan> procArray) {
            PrintProcessInfo(i, procName, procArray, FormatReceive<T>());
        }
        public void PrintSending<T>(int i, string procName,  List<PostMan> procArray) {
            PrintProcessInfo(i, procName, procArray, FormatSend<T>());
        }

        public string FormatSend<T>() {
            return string.Format("Attempting to send message of type {0}",  typeof(T).Name);
        }
        public string FormatReceive<T>() {
            return string.Format("Expecting to receive message of type {0}", typeof(T).Name);
        }       
    }
}
