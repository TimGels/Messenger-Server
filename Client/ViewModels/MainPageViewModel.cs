using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Group = Messenger_Client.Models.Group;


namespace Messenger_Client.ViewModels
{
    public class MainPageViewModel : ObservableRecipient, INotifyPropertyChanged
    {
        public ICommand SendMessageCommand { get; set; }
        public ICommand CheckEnterCommand { get; set; }
        public ICommand AddGroupCommand { get; set; }
        public ICommand SignUpCommand { get; set; }
        public ObservableCollection<Group> GroupList
        {
            get
            {
                return Client.Instance.Groups;
            }
        }
        public ObservableCollection<Message> MessagesList
        {
            get
            {

                if (this.SelectedGroupChat == null)
                {
                    return null;
                }
                else
                {
                    return SelectedGroupChat.Messages;
                }
            }
            set { }
        }

        private Group selectedGroupChat;

        public Group SelectedGroupChat
        {
            get { return selectedGroupChat; }
            set { selectedGroupChat = value;
                OnPropertyChanged("MessagesList");
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
            if (this.TypedText.Equals(""))
            {
                return;
            }

            Message message = new Message()
            {
                MessageType = MessageType.ChatMessage,
                ClientId = Client.Instance.Id,
                GroupID = SelectedGroupChat.Id,
                PayloadData = this.TypedText
            };

            Client.Instance.Connection.SendData(message);
            SelectedGroupChat.AddMessage(message);
            this.TypedText = "";
        }

        public void CheckEnterPressed(object args)
        {
            KeyRoutedEventArgs keyargs = (KeyRoutedEventArgs)args;
            if (keyargs.Key == Windows.System.VirtualKey.Enter)
            {
                SendMessage();
            }
        }

        private void OpenAddGroupView()
        {
            Debug.WriteLine("OpenAddGroupView");
            (Window.Current.Content as Frame).Navigate(typeof(LoginPage));

        }

        private void OpenSignUpView()
        {
            Debug.WriteLine("OpenSignUpView");
        }

        public MainPageViewModel()
        {
            SendMessageCommand = new RelayCommand(() => SendMessage());
            CheckEnterCommand = new RelayCommand<object>(CheckEnterPressed);
            AddGroupCommand = new RelayCommand(() => OpenAddGroupView());
            SignUpCommand = new RelayCommand(() => OpenSignUpView());

            this.TypedText = "";

            Random rnd = new Random();

            for (int i = 0; i < 20; i++)
            {
                Group group = new Group(i, string.Format("GroupName {0}", i));
                Client.Instance.AddGroup(group);
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
