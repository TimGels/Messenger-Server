using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Messenger_Client.ViewModels
{
    class AddGroupPageViewModel : ObservableRecipient
    {
        public ICommand BackToMainPageCommand { get; set; }
        public ICommand AddGroupCommand { get; set; }
        public ICommand CheckEnterCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        public string NewGroupName { get; set; }

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
            BackToMainPageCommand = new RelayCommand<object>(BackToMain);
            AddGroupCommand = new RelayCommand(AddNewGroup);
            CheckEnterCommand = new RelayCommand<object>(CheckEnterPressed);
            LogoutCommand = new RelayCommand(Logout);
        }

        private void CheckEnterPressed(object obj)
        {
            KeyRoutedEventArgs keyargs = (KeyRoutedEventArgs)obj;
            if (keyargs.Key == Windows.System.VirtualKey.Enter)
            {
                AddNewGroup();
            }
        }

        private void AddNewGroup()
        {
            if (this.NewGroupName != null && !this.NewGroupName.Equals(""))
            {
                CommunicationHandler.SendRegisterGroupMessage(this.NewGroupName);
                CommunicationHandler.RegisterGroupResponse += CommunicationHandler_RegisterGroupResponse;
            }
            else
            {
                CreateGroupErrorMessage = "A group needs a name";
            }
        }

        private async void CommunicationHandler_RegisterGroupResponse(object sender, CommunicationHandler.ResponseStateEventArgs e)
        {
            switch (e.State)
            {
                case -1:
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        CreateGroupErrorMessage = "Group not created";
                    });

                    break;
                default:
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        (Window.Current.Content as Frame).Navigate(typeof(MainPage));
                    });

                    break;
            }
        }

        private void BackToMain(object obj)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MainPage));
        }

        private void Logout()
        {
            //TODO: Logout implementation
            //Client.Instance.Connection.Close();

            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(LoginPage));

            Debug.WriteLine("Logout");
        }
    }
}
