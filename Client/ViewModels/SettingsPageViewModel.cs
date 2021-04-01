using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.ViewModels
{
    class SettingsPageViewModel
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public string IPAddress
        {
            get
            {
                if (this.localSettings.Values["IPAddress"] != null)
                {
                    return this.localSettings.Values["IPAddress"].ToString();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                IPAddress = value;
                this.localSettings.Values["IPAddress"] = value;
            }
        }
        public string PortNumber
        {
            get
            {
                if (this.localSettings.Values["PortNumber"] != null)
                {
                    return this.localSettings.Values["PortNumber"].ToString();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                PortNumber = value;
                this.localSettings.Values["PortNumber"] = value;
            }
        }
        public bool IsTextBoxEnabled { get; set; }
        public ICommand BackButtonCommand { get; set; }


        public SettingsPageViewModel()
        {
            IsTextBoxEnabled = true;
            BackButtonCommand = new RelayCommand(backToLogin);
        }

        private void backToLogin()
        {
            (Window.Current.Content as Frame).Navigate(typeof(LoginPage));
        }
    }
}
