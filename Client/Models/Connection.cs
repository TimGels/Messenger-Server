using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger_Client.Models
{
    public class Connection
    {
        private readonly TcpClient client;

        public Connection(string serverAddress, int port)
        {
            this.client = new TcpClient(serverAddress, port);
            //start readerThread
            new Thread(ReadData).Start();
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
                        Task.Run(() => CommunicationHandler.HandleMessage(new Message(data)));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
            }
        }

        public void SendMessage(Message message)
        {
            string data = Message.SerializeMessage(message);
            Byte[] buffer = Encoding.ASCII.GetBytes(data);
            client.GetStream().Write(buffer, 0, buffer.Length);
        }
    }
}
