using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.ViewModels
{
    public class SettingsPageViewModel : ObservableRecipient
    {
        /// <summary>
        /// Convenience field to store the local appdata container.
        /// </summary>
        private readonly ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        public string IPAddress
        {
            get
            {
                return this.settings.Values["IPAddress"].ToString();
            }
            set
            {
                this.settings.Values["IPAddress"] = value;
                OnPropertyChanged();
            }
        }

        public string PortNumber
        {
            get
            {
                return this.settings.Values["PortNumber"].ToString();
            }
            set
            {
                this.settings.Values["PortNumber"] = value;
                OnPropertyChanged();
            }
        }

        public bool IsTextBoxEnabled
        {
            get
            {
                return !Client.Instance.Connection.IsConnected();
            }
        }

        public bool UsePLINQ
        {
            get
            {
                return bool.Parse(this.settings.Values["UsePLINQ"].ToString());
            }
            set
            {
                this.settings.Values["UsePLINQ"] = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackButtonCommand { get; set; }

        public SettingsPageViewModel()
        {
            BackButtonCommand = new RelayCommand(BackButton);
        }

        private void BackButton()
        {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                }
        }
    }
}
