using System;
using SharedObjects;
using System.ComponentModel;
using System.Linq;

namespace MyUtilities
{
    public class BindableGameProcessData : BindableEventObject {

        // Take advantage of the SetProperty function used in all of the accessors below to only update values that have changed.
        public GameProcessData GameProcessData {
            get { return new GameProcessData {
                    HasUmbrellaRaised = HasUmbrellaRaised,
                    HitPoints         = HitPoints,
                    LifePoints        = HitPoints,
                    ProcessId         = ProcessId,
                    Type              = Type
                };
            }
            set {
                HasUmbrellaRaised = value.HasUmbrellaRaised;
                HitPoints         = value.HitPoints;
                LifePoints        = value.HitPoints;
                ProcessId         = value.ProcessId;
                Type              = value.Type;
            }
        }

        public BindableGameProcessData() {}
        public BindableGameProcessData(GameProcessData data)         { GameProcessData = data;                 }
        public BindableGameProcessData(BindableGameProcessData data) { GameProcessData = data.GameProcessData; }

        // Implicity convert GameProcessData to BindableGameProcessData
        public static implicit operator GameProcessData(BindableGameProcessData data) {
            return data.GameProcessData;
        }
        // Implicity convert BindableGameProcessData to GameProcessData
        public static implicit operator BindableGameProcessData(GameProcessData data) {
            return new BindableGameProcessData(data);
        }

        private int _processId;
        public int ProcessId {
            get { return _processId; }
            set { SetProperty( ref _processId, value); }
        }

        private int _lifePoints;
        public int LifePoints {
            get { return _lifePoints; }
            set { SetProperty( ref _lifePoints, value); }
        }

        private int _hitPoints;
        public int HitPoints {
            get { return _hitPoints; }
            set { SetProperty( ref _hitPoints, value); }
        }

        private ProcessInfo.ProcessType _type;
        public ProcessInfo.ProcessType Type {
            get { return _type; }
            set { SetProperty( ref _type, value); }
        }

        private bool _hasUmbrellaRaised;
        public bool HasUmbrellaRaised {
             get { return _hasUmbrellaRaised; }
            set { SetProperty( ref _hasUmbrellaRaised, value); }
        }
    }
}