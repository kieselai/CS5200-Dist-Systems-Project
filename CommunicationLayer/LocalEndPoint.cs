using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections;
using SharedObjects;
using Utils;

namespace CommunicationLayer {
    public class LocalEndPoint {
        private static List<int> PortsAssigned = new List<int>();
        private static object PortListLock = new object();
        public static void LockedAction_PortsAssigned( Action<List<int>> action) {
            lock (PortListLock) {
                action(PortsAssigned);
            }
        }
        public enum NetworkConnectionType  { Any = 0, TCP = 1, UDP = 2 };
        protected int?       _port;
        protected IPEndPoint _ipEndPoint;
        public int Port {
            get { _port = _port?? GetPort();  return (int)_port;  }
            set { IPEndPoint.Port = value;    _port = value; }
        }

        public IPEndPoint IPEndPoint {
            get { _ipEndPoint = _ipEndPoint?? new IPEndPoint(IPAddress.Any, Port);   return _ipEndPoint; }
            set { _ipEndPoint = value; _port = value.Port; }
        }

        public NetworkConnectionType ConnectionType { get; set; }

        protected int? _minPort; 
        public int MinPort { get { return _minPort?? 10000; } set { _minPort = value; } }

        protected int? _maxPort;
        public int MaxPort { get { return _maxPort?? 12000; } set { _maxPort = value; } }

        public LocalEndPoint() : this(10000, 12000) {}
        public LocalEndPoint(int minPort, int maxPort) : this(NetworkConnectionType.Any, minPort, maxPort) {}
        public LocalEndPoint(NetworkConnectionType connectionType) :  this(connectionType, 10000, 12000) {}
        public LocalEndPoint(NetworkConnectionType connectionType, int minPort, int maxPort) {
            MinPort = minPort;
            MaxPort = maxPort;
            ConnectionType = connectionType;
        }

        ~ LocalEndPoint() {
            LockedAction_PortsAssigned( (portsAssigned) => {
                if( _port != null && portsAssigned.Contains((int)_port)) {
                    portsAssigned.Remove((int)_port);
                }
            });
        }

        // Convert to IPEndpoint
        public static implicit operator IPEndPoint( LocalEndPoint localEp ) {
            return localEp.IPEndPoint;
        }
        // Convert IPEndpoint to LocalEndpoint
        public static implicit operator LocalEndPoint( IPEndPoint ep ) {
            return new LocalEndPoint { Port = ep.Port, IPEndPoint = ep };
        }
        // Convert LocalEndPoint to PublicEndPoint
        public static implicit operator PublicEndPoint(LocalEndPoint localEp) {
            return new PublicEndPoint(localEp.ToString());
        }
        // Convert PublicEndPoint to LocalEndPoint
        public static implicit operator LocalEndPoint( PublicEndPoint publicEp ) {
            return new LocalEndPoint { IPEndPoint = new IPEndPoint(IPAddress.Any, publicEp.Port), Port = publicEp.Port };
        }

        public override string ToString() {
            return "127.0.0.1:"+Port;
        }


        public int GetPort() {
            var p = GetAvailablePort(ConnectionType, MinPort, MaxPort);
            LockedAction_PortsAssigned( (portsAssigned) => {
                portsAssigned.Add(p);
            });
            return p;
        }

        static public int GetAvailablePort( NetworkConnectionType type, int Start = 10000, int End = 12000 ) {
                 if(type == NetworkConnectionType.TCP) return GetAvailableTCPPort();
            else if(type == NetworkConnectionType.UDP) return GetAvailableUDPPort();
            else return GetAvailablePort();
        }

        static public int GetAvailablePort(int Start = 10000, int End = 12000) {
            var range = Enumerable.Range( Start, End );
            var query = from port in Enumerable.Range( Start, End )
                   join tcpPort in GetAvailableTCPPorts(range) on port equals tcpPort
                   join udpPort in GetAvailableUDPPorts(range) on port equals udpPort
                   select port;
            return query.FirstOrDefault();
        }

        static public int GetAvailableUDPPort( int Start = 10000, int End = 12000 ) {
            return GetAvailableUDPPorts(Enumerable.Range( Start, End )).FirstOrDefault();
        }

        static public int GetAvailableTCPPort( int Start = 10000, int End = 12000 ) {
            return GetAvailableTCPPorts( Enumerable.Range( Start, End ) ).FirstOrDefault();
        }

        static public IEnumerable<int> GetAvailableUDPPorts(IEnumerable<int> PortRange=null) {
            return FilterPorts((portsAssigned, portRange)=> portRange.Except(GetActiveUDPPorts()).Except(portsAssigned), PortRange);
        }

        static public IEnumerable<int> GetAvailableTCPPorts(IEnumerable<int> PortRange=null) {
            return FilterPorts((portsAssigned, portRange)=> portRange.Except(GetActiveTCPPorts()).Except(portsAssigned), PortRange);
        }

        static public IEnumerable<int> GetActiveUDPPorts() {
            return from connections in IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners() select connections.Port;
        }

        static public IEnumerable<int> GetActiveTCPPorts() {
            return from connections in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections() select connections.LocalEndPoint.Port;
        }

        static public IEnumerable<int> FilterPorts(Func<List<int>, IEnumerable<int>, IEnumerable<int>> filterAction, IEnumerable<int> portRange=null) {
            if(portRange == null) portRange = Enumerable.Range(10000, 12000);
            IEnumerable<int> available = null;
            LockedAction_PortsAssigned( (portsAssigned) => {
                available = filterAction(portsAssigned, portRange);
            });
            return available;
        }
    }
}
