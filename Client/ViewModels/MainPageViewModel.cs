using Messenger_Client.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Messenger_Client.ViewModels
{
    public class MainPageViewModel
    {
        public  List<TestMessage> MessageList { get; set; }

        //public Client Client { get; set; }

        public MainPageViewModel()
        {
            this.MessageList = new List<TestMessage>();

            //for (int i = 0; i < 30; i++)
            //{

            //    this.MessageList.Add(new TestMessage());
            //}

            List<Group> groupList = new List<Group>();

            Client client = Client.Instance;

            Message message = new Message();

            for (int i = 0; i < 20; i++)
            {
                Group group = new Group("GroupName", i);
                client.AddGroup(group);
            }
        }
    }
}
