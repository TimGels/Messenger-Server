using Messenger_Client.Models;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.ViewModels
{
    class JoinGroupPageViewModel
    {
        public ICommand JoinGroupCommand { get; set; }
        public ICommand AboutDialogCommand { get; set; }

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
            AboutDialogCommand = new RelayCommand(DisplayAboutDialog);
        }

        private void JoinGroup()
        {

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
