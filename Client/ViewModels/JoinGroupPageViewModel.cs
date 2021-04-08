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
    public class JoinGroupPageViewModel
    {
        public ICommand LogoutCommand { get; set; }
        public ICommand AboutDialogCommand { get; set; }
        public ICommand JoinGroupButtonCommand { get; set; }
        public ICommand CancelButtonCommand { get; set; }
        public ICommand ShowSettingsCommand { get; set; }
        public ICommand ShowAddGroupViewCommand { get; set; }
        public ICommand ExportMessageCommand { get; set; }

        public Group GroupToJoin { get; set; }

        public string ListViewVisibility
        {
            get
            {
                if (GroupList.Count == 0)
                {
                    return "Collapsed";
                }
                else
                {
                    return "Visible";
                }
            }
            set
            { }
        }

        public string NoGroupsMessageVisibility
        {
            get
            {
                if (GroupList.Count == 0)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
            set { }
        }

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

            GroupList = new ObservableCollection<Group>();

            CommunicationHandler.ObtainedRequestedGroups += OnObtainedRequestedGroups;
            CommunicationHandler.SendRequestGroupsMessages();
        }

        private void SendJoinGroupMessage()
        {
            if (GroupToJoin != null)
            {
                Debug.WriteLine(GroupToJoin.Id);
                CommunicationHandler.JoinedGroup += OnJoinedGroup;
                CommunicationHandler.SendJoinGroupMessage(GroupToJoin.Id);
            }
        }

        private void ShowAddGroupView()
        {
            Helper.NavigateTo(typeof(AddGroupPage));
        }

        private async void ExportMessage()
        {
            await Client.Instance.ExportMessageToFileAsync();
        }
        private void ShowSettings()
        {
            Helper.NavigateTo(typeof(SettingsPage));
        }


        private void OnJoinedGroup(object sender, EventArgs e)
        {
            CommunicationHandler.JoinedGroup -= OnJoinedGroup;
            NavigateToMain();
        }

        private void NavigateToMain()
        {
            Helper.NavigateTo(typeof(MainPage));
        }

        private async void OnObtainedRequestedGroups(object sender, CommunicationHandler.GroupListEventArgs e)
        {
            CommunicationHandler.ObtainedRequestedGroups -= OnObtainedRequestedGroups;

            await Helper.RunOnUIAsync(() => e.Groups.ForEach(group => GroupList.Add(group)));
            Helper.RunOnUI(() =>
            {
                OnPropertyChanged("NoGroupsMessageVisibility");
                OnPropertyChanged("ListViewVisibility");
            });
        }

        private async void DisplayAboutDialog()
        {
            await Helper.AboutDialog().ShowAsync();
        }

        private void Logout()
        {
            Client.Instance.Connection.Close();

            Helper.NavigateTo(typeof(LoginPage));

            Debug.WriteLine("Logout");
        }
    }
}
