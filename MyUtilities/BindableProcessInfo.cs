using System;
using SharedObjects;

namespace MyUtilities
{
    public class BindableProcessInfo : BindableEventObject {
        public ProcessInfo Info {
            get {
                return new ProcessInfo {
                    AliveReties    = AliveReties,
                    AliveTimestamp = AliveTimestamp,
                    EndPoint       = EndPoint,
                    Label          = Label,
                    ProcessId      = ProcessId,
                    Status         = Status,
                    Type           = Type
                };
            }
            set {
                AliveReties    = value.AliveReties;
                AliveTimestamp = value.AliveTimestamp;
                EndPoint       = value.EndPoint;
                Label          = value.Label;
                ProcessId      = value.ProcessId;
                Status         = value.Status;
                Type           = value.Type;
            }
        }

        public BindableProcessInfo() {}
        public BindableProcessInfo(ProcessInfo info)         { Info = info;      }
        public BindableProcessInfo(BindableProcessInfo info) { Info = info.Info; }

        public void Reset() {
            Info = new ProcessInfo {
                Status = SharedObjects.ProcessInfo.StatusCode.NotInitialized,
                Type = Info.Type
            };
        }

        public static implicit operator ProcessInfo(BindableProcessInfo info) {
            return info.Info;
        }

        public static implicit operator BindableProcessInfo(ProcessInfo info) {
            return new BindableProcessInfo(info); 
        }

        private int _processId;
        public  int ProcessId {
            get { return _processId; }
            set { SetProperty( ref _processId, value ); }
        }

        private ProcessInfo.ProcessType _type;
        public  ProcessInfo.ProcessType Type {
            get { return _type; }
            set { SetProperty( ref _type, value ); }
        }
        private PublicEndPoint _endPoint;
        public PublicEndPoint EndPoint {
            get { return _endPoint; }
            set { SetProperty(ref _endPoint, value ); }
        }
        private string _label;
        public string Label {
            get { return _label; }
            set { SetProperty(ref _label, value ); }
        }
        private ProcessInfo.StatusCode _status;
        public ProcessInfo.StatusCode Status {
            get { return _status; }
            set { SetProperty( ref _status, value ); }
        }
        public DateTime? _timeStamp;
        public DateTime? AliveTimestamp {
            get { return _timeStamp; }
            set { SetProperty(ref _timeStamp, value ); }
        }
        public int _aliveReties;
        public int AliveReties {
            get { return _aliveReties; }
            set { SetProperty(ref _aliveReties, value ); }
        }
    }
}