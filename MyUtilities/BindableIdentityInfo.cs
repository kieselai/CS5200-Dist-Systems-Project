using System;
using SharedObjects;

namespace MyUtilities
{
    public class BindableIdentityInfo : BindableEventObject {
        public IdentityInfo Info {
            get {
                return new IdentityInfo {
                    Alias     = Alias,
                    ANumber   = ANumber,
                    FirstName = FirstName,
                    LastName  = LastName
                };
            }
            set {
                Alias     = value.Alias;
                ANumber   = value.ANumber;
                FirstName = value.FirstName;
                LastName  = value.LastName;
            }
        }

        public BindableIdentityInfo() {}
        public BindableIdentityInfo(IdentityInfo info)         { Info = info;      }
        public BindableIdentityInfo(BindableIdentityInfo info) { Info = info.Info; }

        public static implicit operator IdentityInfo(BindableIdentityInfo info) {
            return info.Info;
        }

        public static implicit operator BindableIdentityInfo(IdentityInfo info) {
            return new BindableIdentityInfo(info);
        }
        
        private string _alias;
        public string Alias {
            get { return _alias; }
            set { SetProperty( ref _alias, value ); }
        }
        private string _aNumber;
        public string ANumber {
            get { return _aNumber; }
            set { SetProperty( ref _aNumber, value ); }
        }
        private string _firstName;
        public string FirstName {
            get { return _firstName; }
            set { SetProperty( ref _firstName, value ); }
        }
        private string _lastName;
        public string LastName {
            get { return _lastName; }
            set { SetProperty( ref _lastName, value ); }
        }
    }
}