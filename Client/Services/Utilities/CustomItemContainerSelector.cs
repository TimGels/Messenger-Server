using Shared;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.Services.Utilities
{
    public class CustomItemContainerSelector : StyleSelector
    {
        public Style StyleOwnMessageContainer { get; set; }
        public Style StyleOtherMessageContainer { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            var message = (Message)item;

            //TODO: client ID halen uit client.   
            if (message.ClientId == 1)
            {
                return StyleOwnMessageContainer;
            }
            else
            {
                return StyleOtherMessageContainer;
            }
        }
    }
}
