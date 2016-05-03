using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using SharedObjects;
using Messages;
using log4net;

namespace CommunicationLayer
{
    public abstract class ConversationFactory {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConversationFactory));

        // Store a mapping of message types to conversation types
        private ConcurrentDictionary<Type, Type> typeMap;

        // Default Values
        public int                    DefaultTimeout    { get; set; }
        public int                    DefaultMaxRetries { get; set; }
        public CommunicationSubsystem SubSystem         { get; set; }

        public ConversationFactory() {
            typeMap = new ConcurrentDictionary<Type, Type>();
            DefaultTimeout    = 2000;
            DefaultMaxRetries = 3;
        }

        // Unwrap envelope and call Create using the message
        public IConversation Create( Envelope env ){
            if( env == null ) {
                log.Error("Envelope supplied to conversation factory was null.");
                return null;
            }
            else return Create(env.Message);
        }

        // Double check message to make sure everything seems okay, and then create the message
        public IConversation Create( Message message ) {
            if( message == null ) {
                log.Error("Message supplied to conversation factory was null.");
                return null;
            }
            else {
                var type = message.GetType();
                if( type == typeof(Routing) ) {
                    if( ((Routing)message).InnerMessage == null ) {
                        log.Error("InnerMessage of Routing Message supplied to conversation factory was null.");
                        return null;
                    }
                    else type = ((Routing)message).InnerMessage.GetType();
                }
                if ( typeMap.ContainsKey( type ) )
                    return CreateConversationFromType( typeMap[ type ], message );
                else {
                    log.Error("Factory does not have a mapping for type: " + type );
                    return null;
                }
            }
        }

        // Create From conversation type. The public method is Type checked
        public IConversation Create<T>() where T : IConversation {
            return CreateConversationFromType( typeof(T) );
        }

        // Create From Type and message, and then set default values for that conversation.  
        // Set to private to ensure only IConversation objects can be passed as type
        private IConversation CreateConversationFromType( Type type, Message m = null ) {
            return SetConversationDefaults( ActivatorCreate(type), m );
        }

        // Actually creates the conversation
        private IConversation ActivatorCreate(Type type) {
            log.Debug("Creating Conversation of type "+ type.Name);
            return Activator.CreateInstance( type ) as IConversation;
        }

        protected IConversation SetConversationDefaults( IConversation instance, Message m = null ) {
            instance.ConversationId = m == null? MessageNumber.Create() : m.ConvId;
            instance.RetryLimit     = DefaultMaxRetries;
            instance.TimeoutLimit   = DefaultTimeout;
            instance.SubSystem      = SubSystem;
            return instance;
        }

        protected void Add ( IDictionary<Type, Type> dict ) {
            foreach ( var pair in dict ) {
                Add( pair.Key, pair.Value );
            }
        }

        protected void Add ( Type key, Type val ) {
           typeMap.AddOrUpdate(key, val, (k, v) => v);
        }
    }
}
