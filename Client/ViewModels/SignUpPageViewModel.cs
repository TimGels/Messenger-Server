using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace Messenger_Client.ViewModels
{
    class SignUpPageViewModel : ObservableRecipient
    {
        /// <summary>
        /// Commands for the buttons on the sign up page.
        /// </summary>
        public ICommand GoToLoginButtonCommand { get; set; }
        public ICommand RegisterButtonCommand { get; set; }
        public ICommand SettingsButtonCommand { get; set; }

        /// <summary>
        /// The password repeat input.
        /// </summary>
        public string RepeatPassword { get; set; }

        /// <summary>
        /// The password input.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The e-mail input.
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// The name input.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The sign up error message string.
        /// Initialized as empty string.
        /// </summary>
        private string signUpErrorMessage = "";
        public string SignUpErrorMessage
        {
            get
            {
                return signUpErrorMessage;
            }
            set
            {
                signUpErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public SignUpPageViewModel()
        {
            // Page buttons
            RegisterButtonCommand = new RelayCommand(RegisterButtonClicked);
            GoToLoginButtonCommand = new RelayCommand(GoToLoginButtonClicked);
            SettingsButtonCommand = new RelayCommand(SettingsButtonClicked);
        }

        /// <summary>
        /// Handles what happens when the register button is clicked.
        /// Handles register error messages and sends registration to the communication handler.
        /// </summary>
        private async void RegisterButtonClicked()
        {
            if (Name == null || Name.Equals(""))
            {
                SignUpErrorMessage = "You have to fill in a name";
                return;
            }

            if (Mail == null || Mail.Equals(""))
            {
                SignUpErrorMessage = "You have to fill in an E-mail";
                return;
            }

            if (Password == null || Password.Equals(""))
            {
                SignUpErrorMessage = "You have to fill in a password";
                return;
            }

            if(RepeatPassword == null)
            {
                SignUpErrorMessage = "You have to repeat your password";
                return;
            }

            if (!Password.Equals(RepeatPassword))
            {
                SignUpErrorMessage = "Your password has to be the same";
                return;
            }

            if (await Client.Instance.Connection.OpenAsync() == false)
            {
                SignUpErrorMessage = "Could not connect to the server!";
                return;
            }

            CommunicationHandler.SignUpResponse += OnSignUpResponseReceived;
            CommunicationHandler.SendRegisterMessage(Mail, Name, Password);
        }

        /// <summary>
        /// Navigates to the login page.
        /// </summary>
        private void GoToLoginButtonClicked()
        {
            Helper.NavigateTo(typeof(LoginPage));
        }

        /// <summary>
        /// Navigates to the settings page.
        /// </summary>
        private void SettingsButtonClicked()
        {
            Helper.NavigateTo(typeof(SettingsPage));
        }

        /// <summary>
        /// Handles what happens after signing up.
        /// Shows an error message if e-mail is already in use or navigates back to login page when everything has gone well.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Communication handler's response state.</param>
        private void OnSignUpResponseReceived(object sender, CommunicationHandler.ResponseStateEventArgs e)
        {
            CommunicationHandler.SignUpResponse -= OnSignUpResponseReceived;

            switch (e.State)
            {
                case -1:
                    Helper.RunOnUI(() => SignUpErrorMessage = "E-Mail already in use");
                    break;
                default:
                    Helper.NavigateTo(typeof(LoginPage));
                    break;
            }
        }
    }
}
