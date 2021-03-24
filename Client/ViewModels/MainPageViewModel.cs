using Messenger_Client.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Messenger_Client.ViewModels
{
    public class MainPageViewModel : ObservableRecipient, INotifyPropertyChanged
    {
        public List<TestMessage> MessageList { get; set; }

        public List<Group> GroupList { get; set; }

        private Group selectedGroupChat;

        public Group SelectedGroupChat
        {
            get
            {
                return selectedGroupChat;
            }
            set
            {
                selectedGroupChat = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel()
        {

            this.MessageList = new List<TestMessage>();
            this.GroupList = new List<Group>();


            Client client = Client.Instance;

            Random rnd = new Random();

            for (int i = 0; i < 20; i++)
            {
                Group group = new Group(string.Format("GroupName {0}", i), i);
                client.AddGroup(group);
                GroupList.Add(group);
            }


            for (int i = 0; i < 1000; i++)
            {
                int clientId = rnd.Next(0, 2);
                int groupId = rnd.Next(0, 20);
                Message message = new Message()
                {
                    ClientName = "ClientName1",
                    ClientId = clientId,
                    DateTime = DateTime.Now,
                    GroupID = groupId,
                    MessageType = MessageType.ChatMessage,
                    ImageString = "R0lGODlhAQABAIAAAAAAAAAAACH5BAAAAAAALAAAAAABAAEAAAICTAEAOw==",
                    PayloadData = $"this is a message with client {clientId} and group {groupId}",
                };

                GroupList[groupId].AddMessage(message);
            }
        }
    }
}
