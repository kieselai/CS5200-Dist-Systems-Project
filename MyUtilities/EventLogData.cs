using System;
using System.ComponentModel;

namespace MyUtilities {
    public class EventLogData : BindableEventObject {
        protected int _id;
        protected DateTime timeStamp;
        protected string thread, message, logLevel, logger;
        public int Id {
            get { return _id; }
            set { SetProperty( ref _id, value); }
        }
        public DateTime TimeStamp  {
            get { return timeStamp; }
            set { SetProperty( ref timeStamp, value, "TimeString"); }
        }
        public string TimeString {
            get { return TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"); }
        }
        public string Thread {
            get { return thread; }
            set { SetProperty( ref thread, value); }
        }
        public string Message {
            get { return message; }
            set { SetProperty( ref message, value); }
        }
        public string LogLevel {
            get { return logLevel; }
            set { SetProperty( ref logLevel, value); }
        }
        public string Logger {
            get { return logger; }
            set { SetProperty( ref logger, value); }
        }
    }
}
