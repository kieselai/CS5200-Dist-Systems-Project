/* 
    Anthony Kiesel
    Balloon Store Connection Class
*/
using System;
using log4net;
using System.Collections.Generic;

using SharedObjects;
using Messages;
using CommunicationLayer;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System.Threading.Tasks;
using System.Net.Sockets;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace BalloonStore
{
    public class BalloonStoreConnection {
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BalloonStoreConnection));

        public BalloonStoreProcess BalloonStore    { get; set; }
        public bool                IsRunning       { get; set; }

        public BalloonStoreConnection() {
            IsRunning = false;
            BalloonStore = new BalloonStoreProcess();
        }

        public Dictionary<string, string> getLocalSettings() {
            log.Debug("Using Local Settings.");
            var config = Properties.Settings.Default;
            return new Dictionary<string, string> {
                { "Registry",     (string)config["Dev_Registry_addr"    ]},
                { "Alias",        "Akiesel's Balloon Store"              }
            };
        }

        public Dictionary<string, string> getRemoteSettings() {
            log.Debug("Using Remote Settings.");
            var config = Properties.Settings.Default;
            return new Dictionary<string, string> {
                { "Registry",     (string)config["Dev_Registry_addr"    ]},
                { "Alias",        "Akiesel's Balloon Store"              }
            };
        }
        
        public void Start( bool isLocal ) {
            if(isLocal) Start( getLocalSettings() );
            else Start( getRemoteSettings() );
        }

        public void Start( Dictionary<string, string> settings) {
            log.Debug( "Starting Application. " );

            log.Debug( "Initializing Subsystem" );
            BalloonStore.initializeSubsystem( settings[ "Registry" ] );

            log.Debug("Initializing Balloon Store");
            BalloonStore.initializeBalloonStore( settings["Alias"] );
            IsRunning = true;
            BalloonStore.Start();
        }

        public void Stop() {
            log.Debug("Stopping Balloon Store Process.");
            IsRunning = false;
            BalloonStore.Stop();
        }
    }
}