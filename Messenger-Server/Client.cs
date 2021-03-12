using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public class Client
    {
        private readonly TcpClient client;

        public Client(TcpClient client)
        {
            this.client = client;
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
                        Task.Run(() => CommunicationHandler.HandleMessage(new Message(this, data)));
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
