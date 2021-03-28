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

namespace Messenger_Client.ViewModels
{
    class LoginPageViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
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
                CommunicationHandler.LoggedInSuccesfully += communicationHandler_LoggedInSuccesfully;
            }
            else
            {
                //TODO create pop up or something
                Debug.WriteLine("email of password is empty!");
            }
        }

        private async void communicationHandler_LoggedInSuccesfully(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                (Window.Current.Content as Frame).Navigate(typeof(MainPage));
            });
        }

        private void RegisterButtonClicked()
        {
            (Window.Current.Content as Frame).Navigate(typeof(SignUpPage));
        }
    }
}
