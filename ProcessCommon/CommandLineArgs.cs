using CommandLine;
using CommandLine.Text;
using System;
using System.Reflection;
using System.Linq;
using MyUtilities;

namespace ProcessCommon
{
    public class CommandLineArgs {
        private static Properties.Settings Settings { get { return Properties.Settings.Default; } }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CommandLineArgs));
        public bool UseLocalSettings { get; set; }

        [Option("registry", MetaValue = "STRING", HelpText = "Registry EndPoint")]
        public string _registry { get; set; }
        public string Registry {
            get {
                return string.IsNullOrWhiteSpace(_registry)
                    ? ( UseLocalSettings? Settings.Dev_Registry_addr : Settings.Registry_addr )
                    : _registry;
            }
        }

        [Option("label", MetaValue = "STRING", HelpText = "Process Label")]
        public string _label { get; set; }
        public string Label  { get { return string.IsNullOrWhiteSpace(_label)? Settings.Alias : _label; } }

        [Option("anumber", MetaValue = "STRING", HelpText = "Process ANumber")]
        public string _aNumber { get; set; }
        public string ANumber {
            get {
                return string.IsNullOrWhiteSpace(_aNumber)
                    ? ( UseLocalSettings? "A00000001" : Settings.ANumber )
                    : _aNumber;
            }
        }

        [Option("firstname", MetaValue = "STRING", HelpText = "First Name")]
        public string _firstName { get; set; }
        public string FirstName {
            get {
                return string.IsNullOrWhiteSpace(_firstName)
                    ? ( UseLocalSettings? "The" : Settings.FirstName )
                    : _firstName;
            }
        }

        [Option("lastname", MetaValue = "STRING", HelpText = "Last Name")]
        public string _lastName { get; set; }
        public string LastName {
            get {
                return string.IsNullOrWhiteSpace(_lastName)
                    ? ( UseLocalSettings? "Instructor" : Settings.LastName )
                    : _lastName;
            }
        }

        [Option("alias", MetaValue = "STRING", HelpText = "Process Alias")]
        public string _alias { get; set; }
        public string Alias {
            get {
                return string.IsNullOrWhiteSpace(_alias)
                    ? ( UseLocalSettings? "The Instructor" : Settings.Alias )
                    : _alias;
            }
        }

        [Option("minport", MetaValue = "INT", HelpText = "Minimum Port to connect with")]
        public int? _minPort  { get; set; }
        public int  MinPort   { get { return  _minPort?? Settings.MinPort; } }

        [Option("maxport", MetaValue = "INT", HelpText = "Max Port to connect with")]
        public int? _maxPort  { get; set; }
        public int  MaxPort   { get { return _maxPort?? Settings.MaxPort; } }

        [Option("timeout", MetaValue = "INT", HelpText = "Default timeout before retry")]
        public int? _timeout  { get; set; }
        public int  Timeout   { get { return _timeout?? Settings.Timeout; } }

        [Option('s', "autostart", HelpText = "Autostart?")]
        public bool? _autoStart { get; set; }
        public bool AutoStart   { get { return _autoStart?? Settings.AutoStart; } }

        [Option("retries", MetaValue = "INT", HelpText = "Default max retries")]
        public int? _retries { get; set; }
        public int  Retries { get { return _retries?? Settings.Retries; } }

        [HelpOption]
        public string GetUsage() {
            var help = new HelpText();
            help.AddOptions(this);
            return help;
            //return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public static C_Args ProcessOptions<C_Args>() where C_Args : CommandLineArgs, new() {
            var args = Environment.GetCommandLineArgs();
            var Options = new C_Args();

            if (!CommandLine.Parser.Default.ParseArguments(args, Options)) {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }
            if( Options.Registry.Split(':')[0].Trim() == "127.0.0.1" || Options.Registry.Split(':')[0].Trim() == "0.0.0.0") {
                log.Debug("Registry is local!");
                Options = new C_Args();
                Options.UseLocalSettings = true;
                CommandLine.Parser.Default.ParseArguments(args, Options);
            }
            else { log.Debug("Registry is remote!"); }
            typeof(C_Args).GetProperties().Tap((prop) => log.Debug("Setting: " + prop.Name + " = " + prop.GetValue(Options)));
            return Options;
        }
    }
}