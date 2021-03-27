using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Group = Messenger_Client.Models.Group;


namespace Messenger_Client.ViewModels
{
    public class MainPageViewModel : ObservableRecipient, INotifyPropertyChanged
    {
        public ICommand SendMessageCommand { get; set; }
        public ICommand CheckEnterCommand { get; set; }
        public ICommand ShowGroupsCommand { get; set; }
        public List<Models.TestMessage> MessageList { get; set; }
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

        private string TypedTextValue;

        public string TypedText
        {
            get
            {
                return TypedTextValue;
            }
            set
            {
                TypedTextValue = value;
                OnPropertyChanged();
            }
        }

        public void SendMessage()
        {
            Client.Instance.Connection.SendData(new Message()
            {
                MessageType = MessageType.ChatMessage,
                ClientId = Client.Instance.Id,
                GroupID = SelectedGroupChat.Id,
                PayloadData = this.TypedText
            });
            this.TypedText = "";
        }

        public void CheckEnterPressed(object args)
        {
            KeyRoutedEventArgs keyargs = (KeyRoutedEventArgs)args;
            Debug.WriteLine(keyargs.Key);
            if (keyargs.Key == Windows.System.VirtualKey.Enter)
            {
                SendMessage();
            }
        }

        private void ShowGroups(object args)
        {
            
            
        }

        public MainPageViewModel()
        {
            SendMessageCommand = new RelayCommand(() => SendMessage());
            CheckEnterCommand = new RelayCommand<object>(CheckEnterPressed);
            ShowGroupsCommand = new RelayCommand<object>(ShowGroups);

            this.GroupList = new List<Group>();
            this.TypedText = "";

            Client client = Client.Instance;

            Random rnd = new Random();

            for (int i = 0; i < 20; i++)
            {
                Group group = new Group(i, string.Format("GroupName {0}", i));
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
                    PayloadData = $"this is a message with client {clientId} and group {groupId}",
                };

                GroupList[groupId].AddMessage(message);
            }
        }


    }
}
