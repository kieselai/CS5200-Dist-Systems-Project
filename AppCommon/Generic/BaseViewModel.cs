using Prism.Commands;
using System;
using MyUtilities;

namespace AppCommon.Generic
{
    abstract public class BaseViewModel : BindableEventObject {

        public DelegateCommand UnconditionalDelegateCommand( Action action ) {
            return new DelegateCommand(action, () => { return true; });
        }

        public DelegateCommand<T> UnconditionalDelegateCommand<T>( Action<T> action ) {
            return new DelegateCommand<T>(action, (t) => { return true; });
        }
    }
}