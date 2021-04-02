using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.ViewModels
{
    class JoinGroupPageViewModel
    {
        public ICommand JoinGroupCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        private Group selectedChat;

        public Group SelectedChat
        {
            get
            {
                return selectedChat;
            }
            set
            {
                selectedChat = value;
            }
        }

        public ObservableCollection<Group> GroupList { get; set; }

        public JoinGroupPageViewModel()
        {
            JoinGroupCommand = new RelayCommand(() => JoinGroup());
            this.GroupList = new ObservableCollection<Group>();
            LogoutCommand = new RelayCommand(Logout);
        }

        private void JoinGroup()
        {

        }

        private void Logout()
        {
            //TODO: Logout implementation
            //Client.Instance.Connection.Close();

            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(LoginPage));

            Debug.WriteLine("Logout");
        }
    }
}
