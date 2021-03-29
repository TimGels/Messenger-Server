using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Group = Messenger_Client.Models.Group;

namespace Messenger_Client.ViewModels
{
    class JoinGroupPageViewModel
    {
        public Group GroupToJoin { get; set; }

        public ICommand JoinGroupButtonCommand { get; set; }
        public ObservableCollection<Group> GroupList;

        public JoinGroupPageViewModel()
        {
            GroupList = new ObservableCollection<Group>();
            CommunicationHandler.SendRequestGroupMessages();
            CommunicationHandler.ObtainedRequestedGroups += obtainedRequestedGroups;
            JoinGroupButtonCommand = new RelayCommand(SendJoinGroupMessage);
        }

        private void SendJoinGroupMessage()
        {
           if(GroupToJoin != null)
            {
                CommunicationHandler.SendJoinGroupMessage(GroupToJoin.Id);
                CommunicationHandler.JoinedGroup += navigateToMainAsync;
            }
        }

        private async void navigateToMainAsync(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                (Window.Current.Content as Frame).Navigate(typeof(MainPage));
            });
        }

        private void obtainedRequestedGroups(object sender, CommunicationHandler.GroupListEventArgs groups)
        {
            groups.Groups.ForEach(group => GroupList.Add(group));
        }
    }
}
