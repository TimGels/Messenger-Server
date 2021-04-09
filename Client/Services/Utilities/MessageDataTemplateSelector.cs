using Shared;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.Services.Utilities
{
    class MessageDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Template containing styling if your own messages.
        /// </summary>
        public DataTemplate OwnMessageDataTemplate { get; set; }

        /// <summary>
        /// Template containing styling of other people's messages.
        /// </summary>
        public DataTemplate SomeoneElseMessageDataTemplate { get; set; }

        /// <summary>
        /// Selects a message style based on who is sender (client ID). This is used in XAML to apply styles based on logic.
        /// </summary>
        /// <param name="item"></param>
        /// <returns><see cref="DataTemplate"/></returns>
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (typeof(Message) != item.GetType())
            {
                return null;
            }

            Message message = (Message)item;
            if (message.ClientId == Client.Instance.Id)
            {
                return OwnMessageDataTemplate;
            }
            else
            {
                return SomeoneElseMessageDataTemplate;
            }
        }

        /// <summary>
        /// Gets called by XAML style selector.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns>Datatemplate style of message</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
