using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger_Client.Models
{

    public class TestMessage
    {
        public string UserName { get; set; }
        public TestMessage()
        {
            this.UserName = "Username";
        }
    }
}
