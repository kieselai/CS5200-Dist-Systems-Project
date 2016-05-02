using CommandLine;
using CommandLine.Text;

namespace BalloonStoreApp
{
    public class CommandLineOptions {
        [Option("registry", MetaValue = "STRING", Required = false, HelpText = "Registry's end point")]
        public string Registry { get; set; }

        [Option("gmid", MetaValue = "INT", Required = false, HelpText = "Game Manager Id")]
        public int? GameManagerIdNullable { get; set; }
        public int GameManagerId { get { return (GameManagerIdNullable == null) ? 0 : (int)GameManagerIdNullable; } }

        [Option("gameid", MetaValue = "INT", Required = false, HelpText = "Game Id")]
        public int? GameIdNullable { get; set; }
        public int GameId { get { return (GameIdNullable == null) ? 0 : (int)GameIdNullable; } }

        [Option("balloons", MetaValue = "INT", Required = false, HelpText = "Number of balloons")]
        public int? BalloonsNullable { get; set; }
        public int Balloons { get { return (BalloonsNullable == null) ? 0 : (int)BalloonsNullable; } }

        [Option("storeindex", MetaValue = "INT", Required = false, HelpText = "Store Index")]
        public int? StoreIndexNullable { get; set; }
        public int StoreIndex { get { return (StoreIndexNullable == null) ? 0 : (int)StoreIndexNullable; } }

        [Option("label", MetaValue = "STRING", Required = false, HelpText = "Process label")]
        public string Label { get; set; }

        [Option("minport", MetaValue = "INT", Required = false, HelpText = "Min port in a range of possible ports for this process's communications")]
        public int? MinPortNullable { get; set; }
        public int MinPort { get { return (MinPortNullable==null) ? 0 : (int) MinPortNullable; } }

        [Option("maxport", MetaValue = "INT", Required = false, HelpText = "Max port in a range of possible ports for this process's communications")]
        public int? MaxPortNullable { get; set; }
        public int MaxPort { get { return (MaxPortNullable == null) ? 0 : (int)MaxPortNullable; } }

        [Option("timeout", MetaValue = "INT", Required = false, HelpText = "Default timeout for request-reply communications")]
        public int? TimeoutNullable { get; set; }
        public int Timeout { get { return (TimeoutNullable == null) ? 0 : (int)TimeoutNullable; } }

        [Option('s', "autostart", Required = false, HelpText = "Autostart")]
        public bool AutoStart { get; set; }

        [Option("retries", MetaValue = "INT", Required = false, HelpText = "Default max retries for request-reply communications")]
        public int? RetriesNullable { get; set; }
        public int Retries { get { return (RetriesNullable == null) ? 0 : (int)RetriesNullable; } }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public void SetDefaults() { }
    }
}
