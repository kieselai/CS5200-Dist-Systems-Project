using Prism.Mvvm;
using System;
using System.Runtime.CompilerServices;

namespace MyUtilities {
    public class BindableEventObject : BindableBase {
        public BindableEventObject() { }
        public bool SetProperty<T>(T accessor, T value, Action<T> setProp, [CallerMemberName] string propertyName = null) {
            if (object.Equals(accessor, value)) return false;
            setProp(value);
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
