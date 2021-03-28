using Messenger_Client.Models;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Messenger_Client.ViewModels
{
    class JoinGroupPageViewModel
    {
        public ICommand JoinGroupCommand { get; set; }

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
        }

        private void JoinGroup()
        {

        }
    }
}
