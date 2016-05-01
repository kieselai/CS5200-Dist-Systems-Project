/* 
    Anthony Kiesel
    Player Connection Class
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

namespace Player
{
    public class PlayerConnection
    {
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PlayerConnection));

        public PlayerProcess Player    { get; set; }
        public bool          IsRunning { get; set; }

        public PlayerConnection() {
            IsRunning = false;
            Player = new PlayerProcess();
        }

        public Dictionary<string, string> getLocalSettings() {
            log.Debug("Using Local Settings.");
            var config = Properties.Settings.Default;
            return new Dictionary<string, string> {
                { "Registry",     (string)config["Dev_Registry_addr"    ]},
                { "FirstName",    "The"                                 },
                { "LastName",     "Instructor"                          },
                { "Alias",        "The Instructor"                      },
                { "ANumber",      "A00000001"                           }
            };
        }

        public Dictionary<string, string> getRemoteSettings() {
            log.Debug("Using Remote Settings.");
            var config = Properties.Settings.Default;
            return new Dictionary<string, string> {
                { "Registry",     (string)config[ "Registry_addr"    ]},
                { "FirstName",    (string)config[ "FirstName"        ]},
                { "LastName",     (string)config[ "LastName"         ]},
                { "Alias",        (string)config[ "Alias"            ]},
                { "ANumber",      (string)config[ "ANumber"          ]}
            };
        }
        
        public void Start( bool isLocal ) {
            if(isLocal) Start( getLocalSettings() );
            else Start( getRemoteSettings() );
        }

        public void Start( Dictionary<string, string> settings) {
            log.Debug( "Starting Application. " );

            log.Debug( "Initializing Subsystem" );
            Player.initializeSubsystem( settings[ "Registry" ] );

            log.Debug("Initializing Player");
            Player.initializePlayer( settings[ "FirstName" ],   
                                     settings["LastName"   ], 
                                     settings["Alias"      ], 
                                     settings["ANumber"    ]);
            IsRunning = true;
            Player.Start();
        }

        public void Stop() {
            log.Debug("Stopping Player Process.");
            IsRunning = false;
            Player.Stop();
        }
    }
}