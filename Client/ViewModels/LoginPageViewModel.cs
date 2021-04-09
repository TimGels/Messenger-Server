using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Messenger_Client.ViewModels
{
    public class LoginPageViewModel : ObservableRecipient
    {
        //values that need to be filled in when logging in
        public string Email { get; set; }
        public string Password { get; set; }

        //string for the errormessage
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

        /// <summary>
        /// check which button on the view has been pressed
        /// </summary>
        public ICommand LoginButtonCommand { get; set; }
        public ICommand RegisterButtonCommand { get; set; }
        public ICommand SettingsButtonCommand { get; set; }
        /// <summary>
        /// check if the enter key has been pressed
        /// </summary>
        public ICommand CheckEnterCommand { get; set; }

        public LoginPageViewModel()
        {
            this.LoginButtonCommand = new RelayCommand(() => HandleLoginAction());
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

        /// <summary>
        /// executes when enter key has been pressed
        /// calls handlelogin function
        /// </summary>
        /// <param name="args"></param>
        public void CheckEnterPressed(object args)
        {
            KeyRoutedEventArgs keyargs = (KeyRoutedEventArgs)args;
            if (keyargs.Key == Windows.System.VirtualKey.Enter)
            {
                HandleLoginAction();
            }
        }

        /// <summary>
        /// function gets called when someone tries to log in. 
        /// </summary>
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
        /// <summary>
        /// Executes when recieving a loginresponse from the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoginInResponseReceived(object sender, CommunicationHandler.ResponseStateEventArgs e)
        {
            CommunicationHandler.LogInResponse -= OnLoginInResponseReceived;

            switch (e.State)
            {
                case -1:
                    Helper.RunOnUI(() => LoginErrorMessage = "The combination of this e-mail and password does not exist!");
                    break;
                case -2:
                    Helper.RunOnUI(() => LoginErrorMessage = "You're already logged in!");
                    break;
                default:
                    Helper.NavigateTo(typeof(MainPage));
                    break;
            }
        }

        /// <summary>
        /// makes sure you get send to the signUpPage after pressing the signUpButton
        /// </summary>
        private void RegisterButtonClicked()
        {
            (Window.Current.Content as Frame).Navigate(typeof(SignUpPage));
        }
    }
}
