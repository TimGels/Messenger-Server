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
        /// Command for the back button.
        /// </summary>
        public ICommand BackButtonCommand { get; set; }

        /// <summary>
        /// Convenience field to store the local appdata container.
        /// </summary>
        private readonly ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        /// <summary>
        /// The IP address settings input.
        /// </summary>
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

        /// <summary>
        /// The port number settings input.
        /// </summary>
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

        /// <summary>
        /// Handles the switch button to turn PLinq on or off.
        /// </summary>
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

        /// <summary>
        /// Returns if settings can be edited.
        /// Settings can only be edited when the client is not connected.
        /// </summary>
        public bool CanEdit
        {
            get
            {
                return !Client.Instance.Connection.IsConnected();
            }
        }

        public SettingsPageViewModel()
        {
            BackButtonCommand = new RelayCommand(BackButton);
        }

        /// <summary>
        /// Functionality for the back button command.
        /// Navigates back to the previous page if it can.
        /// </summary>
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
