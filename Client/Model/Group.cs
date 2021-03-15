using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_Messenger_Client.Models
{
    public class Group
    {
        public List<Message> messages { get; set; }
        public String GroupName { get; set; }
        public int MyProperty { get; set; }

        public Group()
        {

        }

        public void AddMessage(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
