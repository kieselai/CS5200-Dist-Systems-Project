using AppCommon.Generic;
using System.ComponentModel;
using System.Linq;
using log4net;
using MyUtilities;
using System.Collections.ObjectModel;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Collections.Concurrent;
using Prism.Commands;
using AppCommon.Views;
using System.Windows.Controls;

namespace AppCommon.ViewModels
{
    public class LogViewModel : BaseViewModel {
        public CollectionViewSource               LogSource    { get; set; }
        public ObservableCollection<EventLogData> LogMessages  { get; set; }
        private ConcurrentQueue<EventLogData> FrozenLogUpdates { get; set; }
        private bool _autoScrollBottom;
        private bool _freezeUpdate;
        private bool _updatingLog;
        private readonly DelegateCommand _freezeUnfreeze;
        public DelegateCommand FreezeUnfreeze { get { return _freezeUnfreeze; } }
        
        public bool AutoScrollBottom {
            get { return _autoScrollBottom; }
            set { SetProperty(ref _autoScrollBottom, value); }
        }
            
        public bool FreezeUpdate {
            get { return _freezeUpdate; }
            set { SetProperty(ref _freezeUpdate, value); }
        }

        private LogView View {
            get {  return AppDispatcher.GetView<LogView>("LogRegion"); }
        }
        private DataGrid LogGrid {
            get { return View.LogViewGrid; }
        }

        public LogViewModel() : base() {
            FrozenLogUpdates = new ConcurrentQueue<EventLogData>();
            _updatingLog = false;
            _autoScrollBottom = true;
            AppDispatcher.DispatchUI( () => {
                LogMessages = new ObservableCollection<EventLogData>();
                LogSource = new CollectionViewSource();
                LogSource.Source = LogMessages;
                var appender = LogManager.GetRepository().GetAppenders().Where((a) => a.Name == "EventAppender").FirstOrDefault() as EventAppender;
                if (appender != null) {
                    appender.LogUpdate += UpdateLogCollection;
                }
            });
            _freezeUnfreeze = UnconditionalDelegateCommand(() => {
                _updatingLog = true;
                EventLogData data;
                while(FreezeUpdate == false && _updatingLog == true && FrozenLogUpdates.Count > 0) {
                    var success = FrozenLogUpdates.TryDequeue(out data);
                    if (success && data != null) {
                        AddToLogCollection(data);
                    }
                }
                _updatingLog = false;
            });
            
        }
        public void UpdateLogCollection(object sender, EventArgs eventArgs) {
            var e = eventArgs as EventAppenderEventArgs;
            if(e != null && e.Data != null) {
                if(FreezeUpdate || _updatingLog ) { FrozenLogUpdates.Enqueue(e.Data); }
                else { AddToLogCollection(e.Data); }
            }
        }
        public void AddToLogCollection(EventLogData data) {
            AppDispatcher.DispatchUI(() => {
                LogMessages.Add(data);
                if(LogMessages.Count > 50) {
                    var numToRemove = LogMessages.Count - 50;
                    var oldest = (from el in LogMessages orderby el.Id ascending select el).Take(numToRemove);
                    oldest.Update( (el) => LogMessages.Remove(el) );
                }
                if(AutoScrollBottom) LogGrid.ScrollIntoView(LogMessages[LogMessages.Count - 1]);
            });
        }
    }
}
