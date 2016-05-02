using System.Collections.Generic;
using System.Collections.Concurrent;
using SharedObjects;

namespace CommunicationLayer
{
    public class EndpointLookup {
        ConcurrentDictionary<string, PublicEndPoint> endpointMap;
        public EndpointLookup() {
            endpointMap   = new ConcurrentDictionary<string, PublicEndPoint>();
        }

        public PublicEndPoint this[ string key ] {
            get {
                    PublicEndPoint ep = null;
                    var success = false;
                    while ( endpointMap.ContainsKey( key ) && !success ) {
                        success = endpointMap.TryGetValue( key, out ep );
                    }
                    return ep;
            }
            protected set { Add( key, value ); }
        }

        public bool Exists(string key) {
            return endpointMap.ContainsKey(key);
        }

        public void Add( Dictionary<string, PublicEndPoint> dict ) {
            foreach ( var pair in dict ) 
                Add( pair.Key, pair.Value );
        }

        public void Add( string key, PublicEndPoint val ) {
            while( Exists(key) == false ) {
                endpointMap.TryAdd(key, val);
            }
        }
    }
}
