using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Messenger_Client.ViewModels
{
    public class JoinGroupPageViewModel
    {
        public ICommand LogoutCommand { get; set; }
        public ICommand AboutDialogCommand { get; set; }
        public Group GroupToJoin { get; set; }
        public ICommand JoinGroupButtonCommand { get; set; }
        public ICommand CancelButtonCommand { get; set; }
        public ObservableCollection<Group> GroupList { get; set; }

        public JoinGroupPageViewModel()
        {
            AboutDialogCommand = new RelayCommand(DisplayAboutDialog);
            JoinGroupButtonCommand = new RelayCommand(SendJoinGroupMessage);
            CancelButtonCommand = new RelayCommand(NavigateToMain);
            LogoutCommand = new RelayCommand(Logout);

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

        private void OnJoinedGroup(object sender, EventArgs e)
        {
            CommunicationHandler.JoinedGroup -= OnJoinedGroup;
            NavigateToMain();
        }

        private void NavigateToMain()
        {
            Helper.NavigateTo(typeof(MainPage));
        }

        private async void OnObtainedRequestedGroups(object sender, CommunicationHandler.GroupListEventArgs groups)
        {
            CommunicationHandler.ObtainedRequestedGroups -= OnObtainedRequestedGroups;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                groups.Groups.ForEach(group => GroupList.Add(group));
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

