using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.ViewModels
{
    class JoinGroupPageViewModel
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
            CancelButtonCommand = new RelayCommand(navigateToMain);
            GroupList = new ObservableCollection<Group>();
            LogoutCommand = new RelayCommand(Logout);

            CommunicationHandler.ObtainedRequestedGroups += obtainedRequestedGroups;
            CommunicationHandler.SendRequestGroupMessages();
        }

        private void SendJoinGroupMessage()
        {
            if (GroupToJoin != null)
            {
                Debug.WriteLine(GroupToJoin.Id);
                CommunicationHandler.SendJoinGroupMessage(GroupToJoin.Id);
                CommunicationHandler.JoinedGroup += navigateToMainAsync;
            }
        }

        private async void navigateToMainAsync(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                navigateToMain();
            });
        }

        private void navigateToMain()
        {
            (Window.Current.Content as Frame).Navigate(typeof(MainPage));
        }

        private async void obtainedRequestedGroups(object sender, CommunicationHandler.GroupListEventArgs groups)
        {
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

            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(LoginPage));

            Debug.WriteLine("Logout");
        }
    }
}

