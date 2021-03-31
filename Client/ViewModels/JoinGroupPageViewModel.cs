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
        public ICommand BackToMainPageCommand { get; set; }
        public ObservableCollection<Group> GroupList { get; set; }
        public ICommand ShowAddGroupViewCommand { get; set; }
        public ICommand ShowGroupsToJoinCommand { get; set; }


        public JoinGroupPageViewModel()
        {
            GroupList = new ObservableCollection<Group>();
            CommunicationHandler.SendRequestGroupMessages();
            CommunicationHandler.ObtainedRequestedGroups += obtainedRequestedGroupsAsync;
            
            JoinGroupButtonCommand = new RelayCommand(SendJoinGroupMessage);
            BackToMainPageCommand = new RelayCommand<object>(BackToMain);
            ShowGroupsToJoinCommand = new RelayCommand<object>(ShowGroupsToJoin);
            ShowAddGroupViewCommand = new RelayCommand<object>(ShowAddGroupView);
        }
        private void ShowGroupsToJoin(object args)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(JoinGroupPage));
        }

        private void ShowAddGroupView(object obj)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(AddGroupPage));
        }

        private void BackToMain(object obj)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MainPage));
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

        private async void obtainedRequestedGroupsAsync(object sender, CommunicationHandler.GroupListEventArgs groups)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                groups.Groups.ForEach(group => GroupList.Add(group));
            });
        }
    }
}
