/* 
    Anthony Kiesel
    Player Connection Class
*/

using System.Collections.Generic;
using ProcessCommon;
using System.Linq;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Player
{
    public class PlayerConnection : BaseConnection {
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PlayerConnection));

        public PlayerProcess Player { get { return Process as PlayerProcess; } }

        private Properties.Settings Settings { get { return Properties.Settings.Default; } }

        public PlayerConnection() : base() {
            Process = new PlayerProcess();
        }

        public override Dictionary<string, string> getLocalSettings() {
            return base.getLocalSettings().Concat(new Dictionary<string, string>{
                { "FirstName", "The"            },
                { "LastName",  "Instructor"     },
                { "Alias",     "The Instructor" },
                { "ANumber",   "A00000001"      }
            }).ToDictionary(d=>d.Key, d=>d.Value);
        }

        public override Dictionary<string, string> getRemoteSettings() {
            return base.getLocalSettings().Concat(new Dictionary<string, string>{
                { "FirstName", Settings.FirstName },
                { "LastName",  Settings.LastName  },
                { "Alias",     Settings.Alias     },
                { "ANumber",   Settings.ANumber   }
            }).ToDictionary(d=>d.Key, d=>d.Value);
        }

        public override void Start( Dictionary<string, string> settings) {
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
    }
}