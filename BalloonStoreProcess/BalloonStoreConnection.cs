/* 
    Anthony Kiesel
    Balloon Store Connection Class
*/
using System.Collections.Generic;
using ProcessCommon;
using System.Linq;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace BalloonStoreProcess
{
    public class BalloonStoreConnection : BaseConnection {
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BalloonStoreConnection));

        public BalloonStoreProcess BalloonStore    { get { return Process as  BalloonStoreProcess; } }

        public BalloonStoreConnection() : base() {
            Process = new BalloonStoreProcess();
        }

        public override Dictionary<string, string> getLocalSettings() {
            var config = Properties.Settings.Default;
            return base.getLocalSettings().Concat( new Dictionary<string, string> {
                { "Alias", "Akiesel's Balloon Store" },
                { "Index", "1" }
            }).ToDictionary(d=>d.Key, d=>d.Value);
        }

        public override Dictionary<string, string> getRemoteSettings() {
            log.Debug("Using Remote Settings.");
            var config = Properties.Settings.Default;
             return base.getRemoteSettings().Concat( new Dictionary<string, string> {
                { "Alias", "Akiesel's Balloon Store" }
            }).ToDictionary(d=>d.Key, d=>d.Value);
        }

        public override void Start( Dictionary<string, string> settings) {
            log.Debug( "Starting Application. " );

            log.Debug( "Initializing Subsystem" );
            BalloonStore.initializeSubsystem(settings[ "Registry" ]);

            log.Debug("Initializing Balloon Store");
            BalloonStore.initializeBalloonStore(settings["Alias"], int.Parse(settings["Index"]));
            IsRunning = true;
            BalloonStore.Start();
        }
    }
}