using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Messenger_Client.ViewModels
{
    public class JoinGroupPageViewModel : ObservableRecipient
    {
        /// <summary>
        /// Buttons in menubar
        /// </summary>
        public ICommand LogoutCommand { get; set; }
        public ICommand AboutDialogCommand { get; set; }
        public ICommand JoinGroupButtonCommand { get; set; }        
        public ICommand ShowSettingsCommand { get; set; }
        public ICommand ShowAddGroupViewCommand { get; set; }
        public ICommand ExportMessageCommand { get; set; }

        /// <summary>
        /// Cancel joining a group
        /// </summary>
        public ICommand CancelButtonCommand { get; set; }

        /// <summary>
        /// The selected group from the list
        /// </summary>
        public Group GroupToJoin { get; set; }

        /// <summary>
        /// The visibility of the ListView which contains the groups to join.
        /// </summary>
        public string ListViewVisibility
        {
            get
            {
                return GroupList.Count == 0 ? "Collapsed" : "Visible";
            }
            set
            { }
        }

        /// <summary>
        /// If there are no groups to join, a message will appear.
        /// </summary>
        public string NoGroupsMessageVisibility
        {
            get
            {
                return GroupList.Count == 0 ? "Visible" : "Collapsed";                
            }
            set { }
        }

        /// <summary>
        /// All groups in the database.
        /// </summary>
        public ObservableCollection<Group> GroupList { get; set; }

        public JoinGroupPageViewModel()
        {
            // Menubar buttons
            LogoutCommand = new RelayCommand(Logout);
            ShowSettingsCommand = new RelayCommand(ShowSettings);
            ShowAddGroupViewCommand = new RelayCommand(ShowAddGroupView);
            AboutDialogCommand = new RelayCommand(DisplayAboutDialog);
            ExportMessageCommand = new RelayCommand(ExportMessage);

            // Page buttons
            JoinGroupButtonCommand = new RelayCommand(SendJoinGroupMessage);
            CancelButtonCommand = new RelayCommand(NavigateToMain);

            //Create empty group list
            GroupList = new ObservableCollection<Group>();

            // Subscribe on event, this event will be raised when the requested groups are obtained. 
            // Then send the actual message which request the groups from the server.
            CommunicationHandler.ObtainedRequestedGroups += OnObtainedRequestedGroups;
            CommunicationHandler.SendRequestGroupsMessages();
        }

        /// <summary>
        /// Send a request to join GroupToJoin
        /// </summary>
        private void SendJoinGroupMessage()
        {
            if (GroupToJoin != null)
            {
                CommunicationHandler.JoinedGroup += OnJoinedGroup;
                CommunicationHandler.SendJoinGroupMessage(GroupToJoin.Id);
            }
        }

        /// <summary>
        /// Show the AddGroup Page
        /// </summary>
        private void ShowAddGroupView()
        {
            Helper.NavigateTo(typeof(AddGroupPage));
        }

        /// <summary>
        /// Export all the messages to .CSV
        /// </summary>
        private async void ExportMessage()
        {
            await Client.Instance.ExportMessageToFileAsync();
        }

        /// <summary>
        /// Show the settings page
        /// </summary>
        private void ShowSettings()
        {
            Helper.NavigateTo(typeof(SettingsPage));
        }

        /// <summary>
        /// Event to fire after a client joined a group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnJoinedGroup(object sender, EventArgs e)
        {
            CommunicationHandler.JoinedGroup -= OnJoinedGroup;
            NavigateToMain();
        }

        /// <summary>
        /// Show the main page
        /// </summary>
        private void NavigateToMain()
        {
            Helper.NavigateTo(typeof(MainPage));
        }

        /// <summary>
        /// Refreshes the grouplist and changes visibilities after the grouplist is obtained
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnObtainedRequestedGroups(object sender, CommunicationHandler.GroupListEventArgs e)
        {
            CommunicationHandler.ObtainedRequestedGroups -= OnObtainedRequestedGroups;

            await Helper.RunOnUIAsync(() =>
            {
                e.Groups.ForEach(group => GroupList.Add(group));
                OnPropertyChanged("NoGroupsMessageVisibility");
                OnPropertyChanged("ListViewVisibility");
            });
        }

        /// <summary>
        /// Show aboutbox
        /// </summary>
        private async void DisplayAboutDialog()
        {
            await Helper.AboutDialog().ShowAsync();
        }

        /// <summary>
        /// Log out the client
        /// </summary>
        private void Logout()
        {
            Client.Instance.Connection.Close();
            Helper.NavigateTo(typeof(LoginPage));
        }
    }
}
