using log4net;
using System;
using System.Threading;

namespace MyUtilities
{
    public static class ThreadUtil {
        private static readonly ILog log = LogManager.GetLogger(typeof(ThreadUtil));
        public static void Run(Action action) {
            ThreadPool.QueueUserWorkItem(ThreadFunc, action);
        }
        public static void RunAfterDelay(Action action, int delay=500) {
            Run(()=> {
                Thread.Sleep(delay);
                action();
            });
        }
        public static void ThreadFunc(object userFunc) {
            var action = userFunc as Action;
            var func = userFunc as Func<object>;
            if (action != null) {
                action();
            }
            else if(func != null) {
                func();
                func();
            }
            else {
                log.Error("userFunc was: " + userFunc.GetType());
                log.Error("userFunc was an unexpected type.");   
            }
        }
    }
}
