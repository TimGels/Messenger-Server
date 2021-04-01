using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Messenger_Client.ViewModels
{
    public class LoginPageViewModel : ObservableRecipient
    {
        public string Email { get; set; }
        public string Password { get; set; }

        private string loginErrorMessage = "";

        public string LoginErrorMessage
        {
            get
            {
                return loginErrorMessage;
            }
            set
            {
                loginErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterButtonCommand { get; set; }
        public ICommand LoginButtonCommand { get; set; }
        public ICommand CheckEnterCommand { get; set; }

        public LoginPageViewModel()
        {
            this.RegisterButtonCommand = new RelayCommand(() => RegisterButtonClicked());
            this.LoginButtonCommand = new RelayCommand(() => LoginButtonClicked());
            this.CheckEnterCommand = new RelayCommand<object>(CheckEnterPressed);
            this.Email = "";
            this.Password = "";
            //to be sure the client is reading when logging in..
            Client client = Client.Instance;
        }

        public void CheckEnterPressed(object args)
        {
            KeyRoutedEventArgs keyargs = (KeyRoutedEventArgs)args;
            if (keyargs.Key == Windows.System.VirtualKey.Enter)
            {
                HandleLoginAction();
            }
        }

        private void LoginButtonClicked()
        {
            HandleLoginAction();
        }

        private void HandleLoginAction()
        {
            if (!Email.Equals("") && !Password.Equals(""))
            {

                CommunicationHandler.SendLoginMessage(Email, Password);
                CommunicationHandler.LoggedInResponse += CommunicationHandler_LoggedInResponse;
            }
            else
            {
                LoginErrorMessage = "E-mail of wachtwoord is niet ingevuld!";
            }
        }

        private async void CommunicationHandler_LoggedInResponse(object sender, CommunicationHandler.ResponseStateEventArgs e)
        {
            switch (e.State)
            {
                case -1:
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        LoginErrorMessage = "E-mail of wachtwoord verkeerd!";
                    });
                    break;
                case -2:
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        LoginErrorMessage = "Je bent al ingelogd!";
                    });
                    break;
                default:
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        (Window.Current.Content as Frame).Navigate(typeof(MainPage));
                    });
                break;
            }
        }

        private void RegisterButtonClicked()
        {
            (Window.Current.Content as Frame).Navigate(typeof(SignUpPage));
        }
    }
}
