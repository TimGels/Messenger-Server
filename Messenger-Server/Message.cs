using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public class Message
    {
        //TODO dateTime.
        public String Payload { get; set; }
        public Client Sender { get; set; }
        public Group Group { get; set; }


        public Message(String payload, Client sender, Group group)
        {
            this.Payload = payload;
            this.Sender = sender;
            this.Group = group;
        }
    }
}
