using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_Messenger_Client.Models
{
    public class Message
    {
        private int groupID;
        private String sender;
        private DateTime dateTime;
        private String payload;

        public Message(int groupID, String sender, DateTime dateTime, String payload)
        {
            this.groupID = groupID;
            this.sender = sender;
            this.dateTime = dateTime;
            this.payload = payload;
        }
    }
}