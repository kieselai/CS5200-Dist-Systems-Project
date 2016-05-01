﻿using Prism.Commands;
using PlayerApp.Generic;
using PlayerApp.Views;

namespace PlayerApp.ViewModels
{
    public class LoginViewModel : BaseViewModel {
        private readonly DelegateCommand _initiateLogin;
        private bool isLocal;
        public bool IsLocal {
            get { return isLocal; }
            set { SetProperty(ref isLocal, value); }
        }
        public DelegateCommand InitiateLogin { get { return _initiateLogin; } }
        public LoginViewModel() : base("Log In") {
            _initiateLogin = TrueDelegateCommand( ()=> {
                if( AppState.Connection.IsRunning == false ) {
                    AppState.Connection.Start( isLocal );
                    Navigate<StatusView>("MainRegion");
                }
            });
        }
    }
}