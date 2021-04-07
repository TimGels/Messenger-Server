using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;

namespace Messenger_Client.ViewModels
{
    class AddGroupPageViewModel : ObservableRecipient
    {
        public ICommand BackToMainPageCommand { get; set; }
        public ICommand AddGroupCommand { get; set; }
        public ICommand CheckEnterCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand AboutDialogCommand { get; set; }

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
            BackToMainPageCommand = new RelayCommand(NavigateToMain);
            AddGroupCommand = new RelayCommand(AddNewGroup);
            CheckEnterCommand = new RelayCommand<KeyRoutedEventArgs>(CheckEnterPressed);
            LogoutCommand = new RelayCommand(Logout);
            AboutDialogCommand = new RelayCommand(DisplayAboutDialog);
        }

        private void CheckEnterPressed(KeyRoutedEventArgs keyargs)
        {
            if (keyargs.Key == Windows.System.VirtualKey.Enter)
            {
                AddNewGroup();
            }
        }

        private void NavigateToMain()
        {
            Helper.NavigateTo(typeof(MainPage));
        }

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

        private async void OnRegisterGroupResponseReceived(object sender, CommunicationHandler.ResponseStateEventArgs e)
        {
            CommunicationHandler.RegisterGroupResponse -= OnRegisterGroupResponseReceived;

            switch (e.State)
            {
                case -1:
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        CreateGroupErrorMessage = "Group not created";
                    });

                    break;
                default:
                    Helper.NavigateTo(typeof(MainPage));
                    break;
            }
        }

        private void Logout()
        {
            Client.Instance.Connection.Close();

            Helper.NavigateTo(typeof(LoginPage));

            Debug.WriteLine("Logout");
        }

        private async void DisplayAboutDialog()
        {
            await Helper.AboutDialog().ShowAsync();
        }
    }
}
