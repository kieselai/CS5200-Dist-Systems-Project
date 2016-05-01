using System;
using log4net.Core;
using System.ComponentModel;

namespace MyUtilities {
    public class EventAppenderEventArgs : EventArgs {
        public EventLogData Data { get; set; }
    }

    public class EventAppender : log4net.Appender.MemoryAppender {
        protected static int _seq;
        protected static int Seq { get {  return _seq++; } }
        public event EventHandler LogUpdate;
        protected override void Append(LoggingEvent e) {
            base.Clear();
            base.Append(e);
            var handler = LogUpdate;
            if( handler != null ) {
                handler(this, new EventAppenderEventArgs {
                    Data = new EventLogData {
                        Id = Seq,
                        TimeStamp = e.TimeStamp,
                        LogLevel  = e.Level.ToString(),
                        Logger    = e.LoggerName,
                        Message   = e.RenderedMessage,
                        Thread    = e.ThreadName
                    }
                });
            }
        }
    }
}
