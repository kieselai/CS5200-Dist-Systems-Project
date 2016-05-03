/* 
    Anthony Kiesel
    Balloon Store Connection Class
*/
using ProcessCommon;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace BalloonStoreProcess
{
    public class BalloonStoreLauncher : BaseLauncher {
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BalloonStoreLauncher));

        public     BalloonStoreProcess    BalloonStore       { get { return Process            as  BalloonStoreProcess; } }
        protected  BalloonStoreState      BalloonStoreState  { get { return BalloonStore.State as BalloonStoreState;    } }
        public new BalloonCommandLineArgs Options            { get { return _options as BalloonCommandLineArgs;  }  set { _options = value; } }

        public BalloonStoreLauncher() : base() {
            Options = CommandLineArgs.ProcessOptions<BalloonCommandLineArgs>();
            Process = new BalloonStoreProcess(Options.MinPort, Options.MaxPort);
        }

        public override void InitState() {
            base.InitState();
            BalloonStoreState.GameManagerId      = Options.GameManagerId;
            BalloonStoreState.GameId             = Options.GameId;
            BalloonStoreState.StartingBalloons   = Options.Balloons;
            BalloonStoreState.StoreIndex         = Options.StoreIndex;
            BalloonStoreState.IdentityInfo.Alias = Options.Alias;
        }
    }
}