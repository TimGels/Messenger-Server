using Messenger_Client.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.Views
{
    public sealed partial class MainPage : Page
    {
        private Group ContextSelectedGroup { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Grid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            ListViewItem listView = (ListViewItem)sender;
            MenuFlyoutContext.ShowAt(listView, e.GetPosition(listView));
            ContextSelectedGroup = (Group)((FrameworkElement)e.OriginalSource).DataContext;
        }

        /// <summary>
        /// Executes a leave group command in the viewmodel with a group object parameter. Could not be done in XAML.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeaveGroupSubMenu_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LeaveGroupCommand.Execute(ContextSelectedGroup);
        }
    }
}
