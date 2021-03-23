using Shared;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public class Client
    {
        /// <summary>
        /// The unique ID of the client.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the client.
        /// </summary>
        public string Name { get; set; }
        public string Email { get; set; }

    }
}
