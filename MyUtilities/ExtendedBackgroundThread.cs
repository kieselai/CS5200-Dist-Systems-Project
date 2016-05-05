using log4net;
using System;
using System.Threading;
using Utils;
namespace MyUtilities
{
    abstract public class ExtendedBackgroundThread : BackgroundThread {
        private static readonly ILog log = LogManager.GetLogger(typeof(ExtendedBackgroundThread));
        public new string Label {
            get { return string.IsNullOrWhiteSpace(base.Label)? GetType().Name : base.Label; }
            set { base.Label = value; }
        }
        public void Start(object state) {
            try {
                KeepGoing = true;
                Suspended = false;
                log.Info("Starting " + Label);
                ThreadPool.QueueUserWorkItem(Process, state);
            }
            catch (Exception err) {
                log.Fatal("Aborted exception caught", err);
            }
        }
    }
}
