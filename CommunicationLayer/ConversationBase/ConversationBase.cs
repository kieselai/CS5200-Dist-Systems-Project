using SharedObjects;
using Messages;
using Utils;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Net;

namespace CommunicationLayer
{
    public interface IConversation {
        void Execute ();
        bool                   HasStarted        { get; set; }
        bool                   HasCompleted      { get; set; }
        bool                   Success           { get; set; }
        bool                   Failure           { get; set; }
        int                    TimeoutLimit      { get; set; }
        int                    RetryLimit        { get; set; }
        MessageNumber          ConversationId    { get; set; }
        ConversationQueue      ConversationQueue { get; set; }
        CommunicationSubsystem SubSystem         { get; set; }
    }

    abstract public class Conversation : BackgroundThread, IConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Conversation));
        private bool _success, _failure;

        public ConversationQueue      ConversationQueue { get; set; }
        public bool                   HasStarted        { get; set; }
        public bool                   HasCompleted      { get; set; }
        public int                    TimeoutLimit      { get; set; }
        public int                    RetryLimit        { get; set; }
        public MessageNumber          ConversationId    { get; set; }
        public CommunicationSubsystem SubSystem         { get; set; }


        public bool Success {
            get { return _success; }
            set {
                _success = value;
                if(value == true) {
                    HasCompleted = true;
                    log.Info(Label + "Conversation Succeeded");
                }
            }
        }
        public bool Failure {
            get { return _failure; }
            set {
                _failure = value;
                if(value == true) {
                    HasCompleted = true;
                    log.Info(Label + "Conversation Failed");
                }
            }
        }
        public string GetLabel {
            get { return this.GetType().Name + ", ConversationID: " + ConversationId + ": "; }
        }

        public Conversation() {
            HasStarted = false; 
            HasCompleted = false;
            Failure = false;
            Success = false;
        }

        public void Execute() {
            Start();
        }

        protected bool SendEnvelope( Envelope env, bool isInitiated ) {
            SetMessageIds(ref env, isInitiated);
            if( env.IsValid() ) {
                log.Debug(GetLabel + "Conversation is initiating send.");
                var retries = 0;
                var successful = false;
                while ( retries < RetryLimit && !successful ) {
                    if( retries != 0 ) { log.Debug(GetLabel + "Last send unsuccessful, trying again."); }
                    successful = SubSystem.PostMan.Send( env, TimeoutLimit );
                    retries += 1;
                }
                if(!successful) { log.Error(GetLabel + "Send was unsuccessful."); }
                else log.Debug(GetLabel + "Send succeeded.");
                return successful;
            }
            else {
                log.Error(GetLabel + env.InvalidMessage() );
                return false;
            }
        }

        protected void SetMessageIds(ref Envelope env, bool isInitiated) {
            log.Debug(GetLabel + "Setting Message IDs");
            env.Message.ConvId = ConversationId;
            env.Message.MsgId = isInitiated? ConversationId : MessageNumber.Create();
            if (env.Message.GetType() == typeof(Routing)) {
                ((Routing)env.Message).InnerMessage.ConvId = ConversationId;
                ((Routing)env.Message).InnerMessage.MsgId  = env.Message.MsgId;
            }
        }

        protected Envelope AddressTo(Message m, PublicEndPoint ep) {
            return new Envelope(m, ep);
        }

        protected Envelope AddressTo(Message m, string lookup) {
            return new Envelope(m, SubSystem.EndpointLookup[lookup]);
        }

        protected Envelope RouteTo(Message innerMessage, params int[] PIDs) {
            log.Debug(GetLabel + "Message is routing message");
            if ( innerMessage != null ){
                return AddressTo(new Routing {
                    InnerMessage = innerMessage, 
                    ToProcessIds = PIDs
                }, "Proxy");
            }
            else {  log.Error(GetLabel + "Inner Message is null"); }
            return null;
        }

        protected T Cast<T>( Envelope envelope ) where T : Message {
            if ( envelope == null ) {
                return null;
            }
            else if( envelope.Message.GetType() == typeof(Routing) ) {
                return (envelope.Message as Routing).InnerMessage as T;
            }
            return envelope.Message as T;
        }

         protected bool MessageIsAvailable() {
            if ( ConversationQueue.Count > 0) {
                log.Debug(GetLabel + "Message available!");
                return true;
            }
            return false;
        }

        protected bool CheckForIncomingMessage() {
            return SyncUtils.WaitForCondition( ()=>MessageIsAvailable(), 4000, 50);
        }

        protected Envelope RetrieveNext() {
            if ( MessageIsAvailable() ) {
                var result = ConversationQueue.Dequeue();
                if( result.IsValid() ) {
                    log.Debug(GetLabel + "Received Message is valid");
                    log.Debug("Received a Message of type: " + result.Message.GetType().Name);
                    if(result.GetType() == typeof(Routing) ) {
                        log.Debug("Inner message is of type: " + ( result.Message as Routing).InnerMessage.GetType().Name);
                    }
                    return result;
                }
                else {
                    log.Error(GetLabel + "Received Message is invalid");
                    log.Error(GetLabel + result.InvalidMessage() );
                    return null;
                }
            }
            return null;
        }

        protected bool ConversationLoop(Func<bool> loopAction, int? customTimeout = null, int? customRetries = null) {
            var timeout    = customTimeout?? TimeoutLimit;
            var numRetries = customRetries?? RetryLimit;
            var step = timeout / 10;
            bool successful = false;
            int remainingTime = timeout;
            var retriesLeft = numRetries;
            do {
                if( HasCompleted == false && successful == false && ( MessageIsAvailable() ||  remainingTime < 0 )) {
                    successful = loopAction();
                    if( HasCompleted ) {
                        return successful;
                    }
                    retriesLeft = successful? numRetries : retriesLeft - 1;
                    remainingTime = (retriesLeft > 0? timeout : -1);
                }
                Thread.Sleep(step);
                remainingTime -= step;
            } while (retriesLeft > 0 && HasCompleted == false && KeepGoing);
            return retriesLeft < 0? false : successful;
        }

        virtual protected void MessageFailure(string failMessage = "") {
            if( !string.IsNullOrWhiteSpace(failMessage)) {
                log.Error(GetLabel + failMessage);
            }
            log.Error(GetLabel + "Conversation Failed");
            log.Error(GetLabel + "Conversation Ending");
            Success = false;
            Failure = true;
            KeepGoing = false;
            Stop();
        }
    }

    abstract public class RequestReplyConversation : Conversation {
        public Envelope IncomingMessage  { get; set; }
        public Envelope OutgoingMessage    { get; set; }

        public bool SetIncomingMessage() {
            if(CheckForIncomingMessage()) {
                IncomingMessage = RetrieveNext();
                return IncomingMessage != null;
            }
            return false;
        }
    }

    abstract public class InitiatedConversation : RequestReplyConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(InitiatedConversation));
        
        abstract protected bool CreateRequest();
        abstract protected bool ProcessReply();
        

        protected override void Process(object state) {
            var success = ConversationLoop(
                ()=> CreateRequest() && InitiateConversation() && SetIncomingMessage() && ProcessReply() );
            if(success || Success) Success = true;
            else MessageFailure();
        }

        protected bool InitiateConversation() {
            log.Debug(GetLabel + "Conversation Id  for Initiated conversation: "+ ConversationId);
            if( SendEnvelope( OutgoingMessage, true ) == false ) {
                return false;
            }
            return true;
        }
    }

    abstract public class ResponseConversation : RequestReplyConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ResponseConversation));
        abstract protected bool ProcessRequest();
        abstract protected bool CreateResponse();
        

        protected override void Process(object state) {
            var success = ConversationLoop(
                ()=> SetIncomingMessage() && ProcessRequest() && CreateResponse() && Respond(OutgoingMessage));
            if(success || Success) Success = true;
            else MessageFailure();
        }

        protected bool Respond(Envelope env) {
            log.Debug(GetLabel + "Conversation Id  for Response conversation: "+ ConversationId);
            if( SendEnvelope( env, false ) == false ) {
                return false;
            }
            return true;
        }
    }

    abstract public class ReceivedMulticast : Conversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ReceivedMulticast));
        public Envelope IncomingMessage  { get; set; }
        abstract protected bool ProcessIncoming();
        protected bool SetIncomingMessage() {
            if(CheckForIncomingMessage()) {
                IncomingMessage = RetrieveNext();
                return IncomingMessage != null;
            }
            return false;
        }
        protected override void Process(object state) {
            var success = ConversationLoop( ()=> SetIncomingMessage() && ProcessIncoming() );
            if(success || Success) Success = true;
            else MessageFailure();
        }
    }

    abstract public class RequestReplyWithTCP : ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RequestReplyWithTCP));
        abstract protected bool OpenTCPStream();

        protected override void Process(object state) {
            var success = ConversationLoop( ()=> SetIncomingMessage() && ProcessRequest() && OpenTCPStream() && CreateResponse() && Respond(OutgoingMessage) );
            if(success || Success) Success = true;
            else MessageFailure();
        }
    }
}