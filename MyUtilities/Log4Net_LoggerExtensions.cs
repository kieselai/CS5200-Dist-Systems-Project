using log4net;

namespace MyUtilities
{
    public static class Log4Net_LoggerExtensions {
        public static void Debug(this ILog log, params string[] messages) {
            foreach( var m in messages ) log.Debug(m);
        }

        public static void Info(this ILog log, params string[] messages) {
            foreach( var m in messages ) log.Info(m);
        }

        public static void Warn(this ILog log, params string[] messages) {
            foreach( var m in messages ) log.Warn(m);
        }

        public static void Error(this ILog log, params string[] messages) {
            foreach( var m in messages ) log.Error(m);
        }
    }
}
