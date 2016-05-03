using CommandLine;
using ProcessCommon;

namespace BalloonStoreProcess
{
    public class BalloonCommandLineArgs : CommandLineArgs {

        [Option("gmid", MetaValue = "INT", Required = true, HelpText = "Game Manager Id")]
        public int GameManagerId { get; set; }

        [Option("gameid", MetaValue = "INT", Required = true, HelpText = "Game Id")]
        public int  GameId { get; set; }

        [Option("balloons", MetaValue = "INT", Required = true, HelpText = "Number of balloons available")]
        public int  Balloons { get; set; }

        [Option("storeindex", MetaValue = "INT", Required = true, HelpText = "Balloon Store Index")]
        public int  StoreIndex { get; set; }

        public new string Alias {
            get {
                return (string.IsNullOrWhiteSpace(_alias) ? ( UseLocalSettings? "The Instructor" : base.Alias ) : _alias) + "'s Balloon Store #" + StoreIndex;
            }
        }
    }
}