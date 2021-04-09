using Messenger_Client.Models;
using Messenger_Client.Services;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Shared;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Input;
using Group = Messenger_Client.Models.Group;

namespace Messenger_Client.ViewModels
{
    public class MainPageViewModel : ObservableRecipient
    {
        /// <summary>
        /// binded to the SendMessageCommand button
        /// </summary>
        public ICommand SendMessageCommand { get; set; }
        /// <summary>
        /// check for the enter key
        /// </summary>
        public ICommand CheckEnterCommand { get; set; }
        /// <summary>
        /// menubar buttons
        /// </summary>
        public ICommand LogoutCommand { get; set; }
        public ICommand AboutDialogCommand { get; set; }
        public ICommand ShowSettingsCommand { get; set; }
        public ICommand ShowAddGroupViewCommand { get; set; }
        public ICommand ShowGroupsToJoinCommand { get; set; }
        public ICommand ExportMessageCommand { get; set; }
        /// <summary>
        /// button to open filepicker when sending an image
        /// </summary>
        public ICommand OpenFilePickerCommand { get; set; }
        /// <summary>
        /// for leaving a group
        /// </summary>
        public ICommand LeaveGroupCommand { get; set; }
        
        /// <summary>
        /// list with all the groups that have been joined
        /// </summary>
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

        /// <summary>
        /// all messages that have to be displayed
        /// </summary>
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

        /// <summary>
        /// groupchat that has been oppened
        /// </summary>
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

        /// <summary>
        /// is the text that has been typed in the textbar
        /// </summary>
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

        /// <summary>
        /// function to leave a group. sends leavegroupmessage to the server
        /// </summary>
        /// <param name="obj"></param>
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

        /// <summary>
        /// opens filepicker and lets you select an image to send in a group
        /// </summary>
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

        /// <summary>
        /// creates a new message with a base64 string as the payloaddata
        /// </summary>
        /// <param name="imageBase64String"></param>
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

        /// <summary>
        /// creates a message with text as the payloaddata
        /// </summary>
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

        /// <summary>
        /// sends a message to the server and adds it to the group
        /// </summary>
        /// <param name="message"></param>
        private void SendMessage(Message message)
        {

            Client.Instance.Connection.SendData(message);
            SelectedGroupChat.AddMessage(message);
            this.TypedText = "";
        }

        /// <summary>
        /// sends a message when the enter key has been pressed
        /// </summary>
        /// <param name="args"></param>
        private void CheckEnterPressed(object args)
        {
            KeyRoutedEventArgs keyargs = (KeyRoutedEventArgs)args;
            if (keyargs.Key == Windows.System.VirtualKey.Enter)
            {
                ConstructTextMessage();
            }
        }

        /// <summary>
        /// opens the settings page
        /// </summary>
        private void ShowSettings()
        {
            Helper.NavigateTo(typeof(SettingsPage));
        }

        /// <summary>
        /// opens the join group page to join an existing group
        /// </summary>
        private void ShowGroupsToJoin()
        {
            Helper.NavigateTo(typeof(JoinGroupPage));
        }

        /// <summary>
        /// opens the add group page to create a new group
        /// </summary>
        private void ShowAddGroupView()
        {
            Helper.NavigateTo(typeof(AddGroupPage));
        }

        /// <summary>
        /// calls function to export all messages to csv
        /// </summary>
        private async void ExportMessage()
        {
            await Client.Instance.ExportMessageToFileAsync();
        }

        /// <summary>
        /// opens the about dialag
        /// </summary>
        private async void DisplayAboutDialog()
        {
            await Helper.AboutDialog().ShowAsync();
        }
    }
}
