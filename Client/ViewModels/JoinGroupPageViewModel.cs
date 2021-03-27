using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Group = Messenger_Client.Models.Group;

namespace Messenger_Client.ViewModels
{
    class JoinGroupPageViewModel : INotifyPropertyChanged
    {
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
                OnPropertyChanged();
            }
        }

    }
}
