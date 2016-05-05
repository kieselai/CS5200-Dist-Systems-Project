using CommunicationLayer;
using log4net;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessCommon;
using SharedObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Utils;
using Common_Proc = ProcessCommon.CommonProcessBase;

namespace TestCommunicationLayer
{
    public static class TestExtensions {
         private static readonly ILog log = LogManager.GetLogger(typeof(TestExtensions));

        public static PublicEndPoint epLookup<T>(this Dictionary<int, T> process, int i, string lookup) where T : Common_Proc {
            return process[i].SubSystem.AddressManager.Lookup[lookup];
        }

        public static LocalEndPoint getEp<T>(this Dictionary<int, T> process, int i) where T : Common_Proc {
            return process[i].SubSystem.PostMan.LocalEndPoint;
        }
        public static PublicEndPoint getEp(this Dictionary<int, PostMan> postman, int i) {
            return postman[i].LocalEndPoint;
        }
        public static LocalEndPoint getLocalEp(this Dictionary<int, PostMan> postman, int i) {
            return postman[i].LocalEndPoint;
        }
        public static ProcessInfo DummyProcessInfo<T>(this Dictionary<int, T> process, int i) where T : Common_Proc {
           return new ProcessInfo {
                AliveReties = 0,
                EndPoint    = process.getEp(i),
                Label       = process[i].Label,
                ProcessId   = 1,
                Status      = ProcessInfo.StatusCode.Registered,
                Type        = ProcessInfo.ProcessType.Player
            };
        }
        public static void SetDummyProcessInfo<T>(this Dictionary<int, T> process, int i) where T : Common_Proc {
            process[i].State.ProcessInfo = process.DummyProcessInfo(i);
        }
        public static void addNew<T>(this Dictionary<int, T> process, int i, DummyGame game) where T : Common_Proc { addNew<T, T>(process, i, game); }
        public static void addNew<T, T2>(this Dictionary<int, T> process, int i, DummyGame game) where T : Common_Proc where T2 : Common_Proc {
            process.Add(i, Activator.CreateInstance(typeof(T2), new object[] { 12000, 20000 }) as T);
            process[i].SubSystem.AddressManager.Lookup.Add("Registry", game.registry.getEp(i));
            process[i].State.IdentityInfo = new IdentityInfo {
                Alias     = "alias"+typeof(T2).Name+"#"+i,
                ANumber   = "anumber",
                FirstName = "first",
                LastName  = "last"
            };
        }

        public static void StoreEp<T>(this T process, string name, PublicEndPoint endpoint) where T : Common_Proc {
            process.SubSystem.AddressManager.Lookup.Add( name, endpoint);
        }
        public static void PrintInfo<T>(this Dictionary<int, T> proc, int i) where T : Common_Proc {
            log.Debug(string.Format(typeof(T).Name+"[{0}], EP: {1}, Time: {2}", i, proc.getEp(i), DateTime.Now));
        }
        public static void PrintInfo(this Dictionary<int, PostMan> proc, int i, string name) {
            log.Debug(string.Format(name+"[{0}], EP: {1}, Time: {2}", i, proc.getEp(i), DateTime.Now));
        }
        public static void PrintSending<T>(this Dictionary<int, T> sender, int i, Envelope env) where T : Common_Proc {
            sender.PrintInfo(i);
            env.PrintSending();
        }
        public static void PrintSending(this Dictionary<int, PostMan> sender, int i, string senderName, Envelope env) {
            sender.PrintInfo(i, senderName);
            env.PrintSending();
        }
        public static void PrintReceive<T, M>(this Dictionary<int, T> receiver, int i) where T : Common_Proc where M : Message {
            receiver.PrintInfo(i);
            PrintReceiving<M>();
        }
        public static void PrintReceive<M>(this Dictionary<int, PostMan> receiver, int i, string receiverName) where M : Message {
            receiver.PrintInfo(i, receiverName);
            PrintReceiving<M>();
        }
        public static void PrintSending(this Envelope env) {
            log.Debug(string.Format("Attempting to send message of type {0}",  env.Message.GetType().Name));
        }
        public static void PrintReceiving<T>() where T : Message {
            log.Debug(string.Format("Expecting to receive message of type {0}", typeof(T).Name));
        }

        public static Envelope SendReceiveWithRetries<ReceiveType>(this Dictionary<int, PostMan> sender, int i, string procName, Envelope env)  where ReceiveType : Message {
            sender.PrintSending(i, procName, env);
            return SendReceiveWithRetries<ReceiveType>(env, sender[i]);
        }

        public static Envelope SendReceiveWithRetries<ReceiveType>(Envelope env, PostMan sender, PostMan receiver = null) where ReceiveType : Message {
            if(receiver == null) receiver = sender;
            Envelope result = null;
            int repeats = 0;
            do {
                ThreadPool.QueueUserWorkItem(SendMessage, Tuple.Create(sender, env));
                result = ReceiveMessageOfType<ReceiveType>(receiver);
                repeats++;
            } while(repeats < 3 && result == null);
            return result;
        }

        public static bool SendWithRetries(this Dictionary<int, PostMan> sender, int i, string procName, Envelope env, Func<bool> SuccessCondition) {
            sender.PrintSending(i, procName, env);
            return sender[i].SendWithRetries(env, SuccessCondition);
        }

        public static bool SendWithRetries(this PostMan sender, Envelope env, Func<bool> SuccessCondition) {
            Envelope result = null;
            int repeats = 0;
            do {
                ThreadPool.QueueUserWorkItem(SendMessage, Tuple.Create(sender, env));
                SyncUtils.WaitForCondition(SuccessCondition, 2500, 50);
                repeats++;
            } while(repeats < 3 && result == null);
            return SuccessCondition();
        }

        public static void SendMessage(object state) {
            var tuple = state as Tuple<PostMan, Envelope>;
            Assert.IsNotNull(tuple);
            var postman = tuple.Item1;
            var env = tuple.Item2;
            postman.Send(env, 1000);
        }
        
        public static Envelope ReceiveMessageOfType<T>(this Dictionary<int, PostMan> proc, int i, string procName) where T : Message {
            proc.PrintInfo(i, procName);
            PrintReceiving<T>();
            return ReceiveMessageOfType<T>(proc[i]);
        }
        

        public static Envelope ReceiveMessageOfType<T>(this PostMan p) where T : Message {
            Envelope result = null;
            var timeout = new TimeSpan(0,0,10);
            var timer = Stopwatch.StartNew();
            do {
                result = p.Receive(1000);
                if(result != null && result.Message != null && result.Message.GetType() != typeof(T)) {
                    log.Debug("Message Received, but expected a message of type '" + typeof(T).Name +"' not '" + result.Message.GetType().Name );
                    result = null;
                }
            } while( result == null && timer.Elapsed < timeout);
            if(result == null) Trace.WriteLine("Test method failed while listening for a message of type: " + typeof(T).Name);
            else Trace.WriteLine("Test method listening for a message of type: " + typeof(T).Name);
            return result;
        }


        public static void StartProcess<T>(this Dictionary<int, T> process, int i, DummyGame game, ProcessInfo.StatusCode state = ProcessInfo.StatusCode.Registered) where T : Common_Proc {
            if ( state != ProcessInfo.StatusCode.Initializing && state != ProcessInfo.StatusCode.NotInitialized) {
                process.SetLoggedIn(i, game);
            }
            process[i].Start();
        }

        public static void StartDispatcherOnly<T>(this Dictionary<int, T> process, int i, DummyGame game, ProcessInfo.StatusCode state = ProcessInfo.StatusCode.Registered) where T : Common_Proc {
            if ( state != ProcessInfo.StatusCode.Initializing && state != ProcessInfo.StatusCode.NotInitialized) {
                process.SetLoggedIn(i, game);
            }
            process[i].SubSystem.Dispatcher.Start();
        }

        public static void SetLoggedIn<T>(this Dictionary<int, T> proc, int i, DummyGame game) where T : Common_Proc {
            proc.SetDummyProcessInfo(i);
            MessageNumber.LocalProcessId = 1;
            proc[i].StoreEp( "Proxy",     game.proxy.getEp(i));
            proc[i].StoreEp( "PennyBank", game.pennyB.getEp(i));
        }

        public static void AddPennies<T>(this Dictionary<int, T> player, int i, int numberOfPennies)  where T : PlayerProcess.PlayerProcess {
            for( var j = 0; j < numberOfPennies; j++) {
                var penny = new Penny { Id = j, SignedBy=10 };
                penny.DigitalSignature = CryptoService.HashAndSign(penny.DataBytes());
                player[i].PlayerState.Pennies.AddOrUpdate(penny);
            }
        }
    }
}
