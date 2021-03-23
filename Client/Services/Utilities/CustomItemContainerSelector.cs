using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messenger_Client.Services.Utilities
{
    public class CustomItemContainerSelector : StyleSelector
    {
        public Style StyleOwnMessage { get; set; }
        public Style StyleOtherMessage { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            var message = (Message)item;
            if (message.ClientId == 1)
            {
                Debug.WriteLine("Own message");
                return StyleOwnMessage;
            }
            else
            {
                Debug.WriteLine("Other message");
                return StyleOtherMessage;
            }
        }
    }
}
