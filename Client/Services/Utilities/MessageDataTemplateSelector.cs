using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.Services.Utilities
{
    class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OwnMessageDataTemplate { get; set; }
        public DataTemplate SomeoneElseMessageDataTemplate { get; set; }

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

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

    }
}
