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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Group = Messenger_Client.Models.Group;


namespace Messenger_Client.ViewModels
{
    public class MainPageViewModel : ObservableRecipient
    {
        public ICommand SendMessageCommand { get; set; }
        public ICommand CheckEnterCommand { get; set; }
        public ICommand AddGroupCommand { get; set; }
        public ICommand SignUpCommand { get; set; }
        public ICommand ShowAddGroupViewCommand { get; set; }
        public ICommand ShowGroupsToJoinCommand { get; set; }
        public ICommand ExportMessageCommand { get; set; }
        public ICommand OpenFilePickerCommand { get; set; }

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
                MessagesList = value.Messages;
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
            ExportMessageCommand = new RelayCommand(ExportMessage);
            
            this.GroupList = new ObservableCollection<Group>();
            this.TypedText = "";
        }

        private async void OpenFilePicker()
        {
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
            Message message = new Message()
            {
                MessageType = MessageType.ChatMessage,
                ClientId = Client.Instance.Id,
                GroupID = SelectedGroupChat.Id,
                DateTime = DateTime.Now,
                PayloadType = "image",
                ClientName = Client.Instance.ClientName,
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

            Message message = new Message()
            {
                MessageType = MessageType.ChatMessage,
                ClientId = Client.Instance.Id,
                GroupID = SelectedGroupChat.Id,
                DateTime = DateTime.Now,
                PayloadType = "text",
                ClientName = Client.Instance.ClientName,
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

        private void ShowGroupsToJoin()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(JoinGroupPage));
        }

        private void ShowAddGroupView()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(AddGroupPage));
        }

        private void OpenSignUpView()
        {
            Debug.WriteLine("OpenSignUpView");
        }

        private async void ExportMessage()
        {
            await Client.Instance.ExportMessageToFileAsync();
        }
    }
}
