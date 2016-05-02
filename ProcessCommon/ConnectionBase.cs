/* 
    Anthony Kiesel
    Base Connection Class
*/

using System.Collections.Generic;
using CommunicationLayer;
using System.Configuration;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ProcessCommon
{
    abstract public class BaseConnection {
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BaseConnection));

        private Properties.Settings Settings { get { return Properties.Settings.Default; } }

        public ICommunicationProcess Process   { get; set; }
        public bool                  IsRunning { get; set; }

        public BaseConnection() {
            IsRunning = false;
        }

        public virtual Dictionary<string, string> getLocalSettings() {
            log.Debug("Using Local Settings.");
            return new Dictionary<string, string> { { "Registry", Settings.Dev_Registry_addr } };
        }

        public virtual Dictionary<string, string> getRemoteSettings() {
            log.Debug("Using Remote Settings.");
            return new Dictionary<string, string> { { "Registry", Settings.Registry_addr } };
        }

        public void Start( bool isLocal ) {
            if(isLocal) Start( getLocalSettings() );
            else Start( getRemoteSettings() );
        }

        public abstract void Start(Dictionary<string, string> settings);

        public virtual void Stop() {
            log.Debug("Stopping " + GetType().Name + " Process.");
            IsRunning = false;
            Process.Stop();
        }
    }
}