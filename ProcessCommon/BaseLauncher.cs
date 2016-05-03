/* 
    Anthony Kiesel
    Base Laucher Class
*/

using CommunicationLayer;
using SharedObjects;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ProcessCommon
{
    abstract public class BaseLauncher {
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BaseLauncher));

        protected CommandLineArgs    _options  { get; set; }
        public    CommandLineArgs    Options   { get { return _options; } set { _options = value; } }
        public CommunicationProcess  Process   { get; set; }
        public bool                  IsRunning { get; set; }

        public BaseLauncher() { IsRunning = false; }

        public virtual void InitState() {
            Process.State.IdentityInfo = new IdentityInfo {
                Alias     = Options.Alias,
                ANumber   = Options.ANumber,
                FirstName = Options.FirstName,
                LastName  = Options.LastName
            };
        }

        public virtual void InitSubSystem() {
            Process.SubSystem.EndpointLookup.Add("Registry", new PublicEndPoint(_options.Registry));
        }

        public virtual void Start() {
            InitState();
            InitSubSystem();
            IsRunning = true;
            Process.Start();
        }

        public virtual void Stop() {
            log.Debug("Stopping " + GetType().Name + " Process.");
            IsRunning = false;
            Process.Stop();
        }
    }
}