using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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

        public ICommand LoginButtonCommand { get; set; }
        public ICommand RegisterButtonCommand { get; set; }
        public ICommand SettingsButtonCommand { get; set; }
        public ICommand CheckEnterCommand { get; set; }

        public LoginPageViewModel()
        {
            this.LoginButtonCommand = new RelayCommand(() => LoginButtonClicked());
            this.RegisterButtonCommand = new RelayCommand(() => RegisterButtonClicked());
            this.SettingsButtonCommand = new RelayCommand(() =>
            {
                (Window.Current.Content as Frame).Navigate(typeof(SettingsPage));
            });
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

        private async void HandleLoginAction()
        {
            if (Email.Equals("") || Password.Equals(""))
            {
                LoginErrorMessage = "Please enter both an email and password!";
                return;
            }

            if (await Client.Instance.Connection.OpenAsync() == false)
            {
                LoginErrorMessage = "Could not connect to the server!";
                return;
            }

            CommunicationHandler.LogInResponse += OnLoginInResponseReceived;
            CommunicationHandler.SendLoginMessage(Email, Password);
        }

        private async void OnLoginInResponseReceived(object sender, CommunicationHandler.ResponseStateEventArgs e)
        {
            CommunicationHandler.LogInResponse -= OnLoginInResponseReceived;

            switch (e.State)
            {
                case -1:
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        LoginErrorMessage = "The combination of this e-mail and password does not exist!";
                    });
                    break;
                case -2:
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        LoginErrorMessage = "You're already logged in!";
                    });
                    break;
                default:
                    Helper.NavigateTo(typeof(MainPage));
                break;
            }
        }

        private void RegisterButtonClicked()
        {
            (Window.Current.Content as Frame).Navigate(typeof(SignUpPage));
        }
    }
}
