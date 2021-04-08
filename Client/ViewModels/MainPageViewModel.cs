using Messenger_Client.Models;
using Messenger_Client.Services;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Shared;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Input;
using Group = Messenger_Client.Models.Group;

namespace Messenger_Client.ViewModels
{
    public class MainPageViewModel : ObservableRecipient
    {
        public ICommand SendMessageCommand { get; set; }
        public ICommand CheckEnterCommand { get; set; }
        public ICommand ShowAddGroupViewCommand { get; set; }
        public ICommand ShowGroupsToJoinCommand { get; set; }
        public ICommand ExportMessageCommand { get; set; }
        public ICommand OpenFilePickerCommand { get; set; }
        public ICommand LeaveGroupCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand AboutDialogCommand { get; set; }
        public ICommand ShowSettingsCommand { get; set; }

        public ObservableCollection<Group> GroupList
        {
            get
            {
                return Client.Instance.Groups;
            }
            set
            {

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
            set
            {
                OnPropertyChanged("MessagesList");
            }
        }

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
                if(value != null)
                {
                    MessagesList = value.Messages;
                }
                else
                {
                    MessagesList = null;
                }
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

        public MainPageViewModel()
        {
            SendMessageCommand = new RelayCommand(ConstructTextMessage);
            CheckEnterCommand = new RelayCommand<object>(CheckEnterPressed);
            ShowGroupsToJoinCommand = new RelayCommand(ShowGroupsToJoin);
            ShowAddGroupViewCommand = new RelayCommand(ShowAddGroupView);
            OpenFilePickerCommand = new RelayCommand(OpenFilePicker);
            LogoutCommand = new RelayCommand(() => Client.Instance.Logout());
            ExportMessageCommand = new RelayCommand(ExportMessage);
            LeaveGroupCommand = new RelayCommand<object>(LeaveGroup);
            AboutDialogCommand = new RelayCommand(DisplayAboutDialog);
            ShowSettingsCommand = new RelayCommand(ShowSettings);

            this.GroupList = new ObservableCollection<Group>();
            this.TypedText = "";
        }

        private void LeaveGroup(object obj)
        {
            if(obj is null)
            {
                return;
            }

            Group group = (Group)obj;
            Client.Instance.RemoveGroup(group);
            CommunicationHandler.SendLeaveGroupMessage(group.Id);
        }

        private async void OpenFilePicker()
        {
            if (this.SelectedGroupChat == null)
            {
                return;
            }

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);

                string imageString = Convert.ToBase64String(await ImageUtilityService.ConvertStreamToByteArray(stream));
                ConstructImageMessage(imageString);
            }
            else
            {
                //this.textBlock.Text = "Operation cancelled.";
            }
        }

        private void ConstructImageMessage(string imageBase64String)
        {
            if (this.SelectedGroupChat == null)
            {
                return;
            }

            Message message = new Message(MessageType.ChatMessage)
            {
                ClientId = Client.Instance.Id,
                GroupID = SelectedGroupChat.Id,
                DateTime = DateTime.Now,
                PayloadType = "image",
                ClientName = Client.Instance.Name,
                PayloadData = imageBase64String
            };
            SendMessage(message);
        }

        private void ConstructTextMessage()
        {
            if (this.TypedText.Equals("") || this.SelectedGroupChat == null)
            {
                return;
            }

            Message message = new Message(MessageType.ChatMessage)
            {
                ClientId = Client.Instance.Id,
                GroupID = SelectedGroupChat.Id,
                DateTime = DateTime.Now,
                PayloadType = "text",
                ClientName = Client.Instance.Name,
                PayloadData = this.TypedText
            };
            SendMessage(message);
        }

        private void SendMessage(Message message)
        {

            Client.Instance.Connection.SendData(message);
            SelectedGroupChat.AddMessage(message);
            this.TypedText = "";
        }

        private void CheckEnterPressed(object args)
        {
            KeyRoutedEventArgs keyargs = (KeyRoutedEventArgs)args;
            if (keyargs.Key == Windows.System.VirtualKey.Enter)
            {
                ConstructTextMessage();
            }
        }

        private void ShowSettings()
        {
            Helper.NavigateTo(typeof(SettingsPage));
        }

        private void ShowGroupsToJoin()
        {
            Helper.NavigateTo(typeof(JoinGroupPage));
        }

        private void ShowAddGroupView()
        {
            Helper.NavigateTo(typeof(AddGroupPage));
        }

        private async void ExportMessage()
        {
            await Client.Instance.ExportMessageToFileAsync();
        }

        private async void DisplayAboutDialog()
        {
            await Helper.AboutDialog().ShowAsync();
        }
    }
}
