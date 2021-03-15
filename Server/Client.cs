using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public class Client
    {
        /// <summary>
        /// The client for which to send and receive data.
        /// </summary>
        private readonly TcpClient client;

        /// <summary>
        /// The unique ID of the client.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the client.
        /// </summary>
        public string Name { get; set; }

        public Client(TcpClient client, int id, string name)
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
                        Task.Run(() => CommunicationHandler.HandleMessage(this, new Message(data)));
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
        /// TODO: check if stream/tcpConnection is writable before writing.
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
