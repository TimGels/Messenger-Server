using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.ViewModels
{
    class SignUpPageViewModel
    {
        public ICommand GoToLoginButtonCommand { get; set; }
        public ICommand RegisterButtonCommand { get; set; }
        public string RepeatPassword { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }

        public SignUpPageViewModel()
        {
            RegisterButtonCommand = new RelayCommand(() => registerButtonClicked());
            GoToLoginButtonCommand = new RelayCommand(() => goToLoginButtonClicked());
        }

        private void goToLoginButtonClicked()
        {
            (Window.Current.Content as Frame).Navigate(typeof(LoginPage));
        }

        private void registerButtonClicked()
        {
            if (Mail.Equals(""))
            {
                Debug.WriteLine("you have to fill in your f******* email!");
                return;
            }

            if (Name.Equals(""))
            {
                Debug.WriteLine("you have to fill in your name");
                return;
            }

            if(Password.Equals("") || !Password.Equals(RepeatPassword))
            {
                Debug.WriteLine("fill your password or do it also in the other box");
                return;
            }

            CommunicationHandler.SendRegisterMessage(Mail, Name, Password);
        }
    }
}
