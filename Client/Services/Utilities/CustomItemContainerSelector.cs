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

            if (message.ClientId == Client.Instance.Id)
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
