using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.Models
{
    public static class Helper
    {
        public static ContentDialog AboutDialog()
        {
            ContentDialog aboutDialog = new ContentDialog
            {
                Title = "About Messenger Vision",
                PrimaryButtonText = "Ok",
                DefaultButton = ContentDialogButton.Primary
            };

            aboutDialog.Content += "Application: Messenger Vision\n";
            aboutDialog.Content += "Version: 1.0\n";
            aboutDialog.Content += "Developers: Jochem Brans, Johannes Kauffmann, Sietze Koonstra, Tim Gels, Rik van Rijn, Ruben Kuilder\n";

            return aboutDialog;
        }

        public static ContentDialog ConnectionLostDialog()
        {
            ContentDialog connectionLostDialog = new ContentDialog
            {
                Title = "Connection lost",
                PrimaryButtonText = "Ok",
                DefaultButton = ContentDialogButton.Primary
            };

            connectionLostDialog.Content += "The connection to the server has been lost. You'll be redirected to the login page.";

            return connectionLostDialog;
        }

        /// <summary>
        /// Navigate to a page. Runs on the UI thread with Normal priority without
        /// awaiting the result.
        /// </summary>
        /// <param name="pageType">The type of page to navigate to.</param>
        public static void NavigateTo(Type pageType)
        {
            RunOnUI(() => (Window.Current.Content as Frame).Navigate(pageType));
        }

        /// <summary>
        /// Run an action on the UI thread with Normal priority without awaiting the result.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void RunOnUI(Action action)
        {
            _ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                action();
            });
        }

        /// <summary>
        /// Run an action on the UI thread with Normal priority and return the result as a
        /// Task. NOTE: uses the CoreDispatcher which returns immediatly. See MS Docs under
        /// "Await a UI task sent from a background thread".
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>Task without the result of the asynchronous operation.</returns>
        public static Task RunOnUIAsync(Action action)
        {
            return CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                action();
            }).AsTask();
        }
    }
}
