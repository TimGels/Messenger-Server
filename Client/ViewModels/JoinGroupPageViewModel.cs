using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Group = Messenger_Client.Models.Group;

namespace Messenger_Client.ViewModels
{
    class JoinGroupPageViewModel
    {
        private Group selectedChat;

        public ObservableCollection<Group> GroupList;

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

        public JoinGroupPageViewModel()
        {
            Debug.WriteLine(GroupList.Count);
        }

    }
}
