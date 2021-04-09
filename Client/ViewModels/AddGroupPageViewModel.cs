using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml.Input;

namespace Messenger_Client.ViewModels
{
    class AddGroupPageViewModel : ObservableRecipient
    {
        /// <summary>
        /// Buttons in menubar
        /// </summary>                 
        public ICommand LogoutCommand { get; set; }
        public ICommand AboutDialogCommand { get; set; }
        public ICommand ShowSettingsCommand { get; set; }
        public ICommand ShowGroupsToJoinCommand { get; set; }
        public ICommand ExportMessageCommand { get; set; }

        /// <summary>
        /// Check if enter is pressed when the cursor is in the textbox with the groupname
        /// </summary>
        public ICommand CheckEnterCommand { get; set; }
        
        /// <summary>
        /// Binded to the add group button
        /// </summary>
        public ICommand AddGroupCommand { get; set; }

        /// <summary>
        /// Binded to the cancel button, cancel adding a group and return to main page
        /// </summary>
        public ICommand BackToMainPageCommand { get; set; }

        /// <summary>
        /// The name of a new group
        /// </summary>
        public string NewGroupName { get; set; }

        /// <summary>
        /// This message is displayed when something went wrong while adding group.
        /// </summary>

        private string createGroupErrorMessage = "";
        public string CreateGroupErrorMessage
        {
            get
            {
                return createGroupErrorMessage;
            }
            set
            {
                createGroupErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public AddGroupPageViewModel()
        {
            // Menubar buttons
            LogoutCommand = new RelayCommand(Client.Instance.Logout);
            ShowSettingsCommand = new RelayCommand(ShowSettings);
            ShowGroupsToJoinCommand = new RelayCommand(ShowGroupsToJoin);
            AboutDialogCommand = new RelayCommand(DisplayAboutDialog);
            ExportMessageCommand = new RelayCommand(ExportMessage);

            // Page buttons
            AddGroupCommand = new RelayCommand(AddNewGroup);            
            BackToMainPageCommand = new RelayCommand(NavigateToMain);

            //Keypressed
            CheckEnterCommand = new RelayCommand<KeyRoutedEventArgs>(CheckEnterPressed);
        }

        /// <summary>
        /// Check if the enter key was pressed. Call AddNewgroup() when true
        /// </summary>
        /// <param name="keyargs"></param>
        private void CheckEnterPressed(KeyRoutedEventArgs keyargs)
        {
            if (keyargs.Key == VirtualKey.Enter)
            {
                AddNewGroup();
            }
        }

        /// <summary>
        /// Navigate to the join group page
        /// </summary>
        private void ShowGroupsToJoin()
        {
            Helper.NavigateTo(typeof(JoinGroupPage));
        }

        /// <summary>
        /// Export all the messages to .CSV
        /// </summary>
        private async void ExportMessage()
        {
            await Client.Instance.ExportMessageToFileAsync();
        }

        /// <summary>
        /// Show the main page
        /// </summary>
        private void NavigateToMain()
        {
            Helper.NavigateTo(typeof(MainPage));
        }

        /// <summary>
        /// Show the settings page
        /// </summary>
        private void ShowSettings()
        {
            Helper.NavigateTo(typeof(SettingsPage));
        }

        /// <summary>
        /// Register a new group
        /// </summary>
        private void AddNewGroup()
        {
            if (this.NewGroupName != null && !this.NewGroupName.Equals(""))
            {
                CommunicationHandler.RegisterGroupResponse += OnRegisterGroupResponseReceived;
                CommunicationHandler.SendRegisterGroupMessage(this.NewGroupName);
            }
            else
            {
                CreateGroupErrorMessage = "A group needs a name";
            }
        }

        /// <summary>
        /// Executes when a registergroup response is received fron the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnRegisterGroupResponseReceived(object sender, CommunicationHandler.ResponseStateEventArgs e)
        {
            CommunicationHandler.RegisterGroupResponse -= OnRegisterGroupResponseReceived;

            switch (e.State)
            {
                case -1:
                    Helper.RunOnUI(() => CreateGroupErrorMessage = "Group not created");
                    break;
                default:
                    Helper.NavigateTo(typeof(MainPage));
                    break;
            }
        }

        /// <summary>
        /// Show aboutbox
        /// </summary>
        private async void DisplayAboutDialog()
        {
            await Helper.AboutDialog().ShowAsync();
        }
    }
}
