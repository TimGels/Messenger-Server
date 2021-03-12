using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public class Client
    {
        private readonly TcpClient client;
        public int Id { get; set; }
        public string Name { get; set; }

        public Client(TcpClient client, string name, int id)
        {
            this.client = client;
            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// Continuously try to read data from the stream. Any incoming message is
        /// handed to a seperate Task, which is responsible for handling the message.
        /// </summary>
        public void ReadData()
        {
            while (true)
            {
                try
                {
                    if (client.GetStream().DataAvailable)
                    {
                        Byte[] buffer = new Byte[client.Available];
                        client.GetStream().Read(buffer, 0, buffer.Length);
                        string data = Encoding.ASCII.GetString(buffer);
                        Message message = new Message(data)
                        {
                            ClientId = this.Id,
                            ClientName = this.Name
                        };
                        Task.Run(() => CommunicationHandler.HandleMessage(this, message));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
            }
        }

        /// <summary>
        /// Send a message to this client.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendData(Message message)
        {
            string data = Message.SerializeMessage(message);
            Byte[] buffer = Encoding.ASCII.GetBytes(data);
            client.GetStream().Write(buffer, 0, buffer.Length);
        }
    }
}
