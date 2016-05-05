/* 
    Anthony Kiesel
    Player Launcher Class
*/

using ProcessCommon;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace PlayerProcess
{
    public class PlayerLauncher : BaseLauncher {
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PlayerLauncher));

        public PlayerLauncher() : base() {
            Options = CommandLineArgs.ProcessOptions<CommandLineArgs>();
            Process = new PlayerProcess(Options.MinPort, Options.MaxPort);
        }

        public override void InitState() {
            base.InitState();
        }
        public override void InitSubSystem() {
            base.InitSubSystem();

        }
    }
}