using Messenger_Client.Services;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        public ICommand AboutDialogCommand { get; set; }

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
            ExportMessageCommand = new RelayCommand<object>(ExportMessage);
            AboutDialogCommand = new RelayCommand(DisplayAboutDialog);
            
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

        private void ExportMessage(object obj)
        {
            Client.Instance.ExportMessageToFileAsync();
        }

        private async void DisplayAboutDialog()
        {
            ContentDialog aboutDialog = new ContentDialog
            {
                Title = "About Messenger Vision",
                PrimaryButtonText = "Ok",
                DefaultButton = ContentDialogButton.Primary
            };

            aboutDialog.Content += "Application: Messenger Vision\n";
            aboutDialog.Content += "Version: 1.0\n";
            aboutDialog.Content += "Developers: Jochem Brans, Johannes Kaufmann, Sietze Koonstra, Tim Gels, Rik van Rijn, Ruben Kuilder\n";

            await aboutDialog.ShowAsync();
        }
    }
}
