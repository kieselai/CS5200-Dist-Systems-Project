using CommunicationLayer;
using System.Net;
using SharedObjects;

namespace BalloonStore
{
    public class BalloonStoreEndpointLookup : EndpointLookup {
        public BalloonStoreEndpointLookup( PublicEndPoint RegistryEp ) {
            Add( "Registry", RegistryEp);
        }
        public BalloonStoreEndpointLookup( string RegistryEp ) {
            Add( "Registry", new PublicEndPoint( RegistryEp ));
        }
    }
}