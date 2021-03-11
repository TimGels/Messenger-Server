using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public class Client
    {
        //private TcpClient client;
        private readonly StreamReader reader;
        private readonly StreamWriter writer;

        public Client(TcpClient client)
        {
            //this.client = client;
            this.reader = new StreamReader(client.GetStream());
            this.writer = new StreamWriter(client.GetStream());
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
                    string data;
                    while ((data = reader.ReadLine()) != null)
                    {
                        Console.WriteLine("Received: " + data);
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
            writer.WriteLine(Message.SerializeMessage(message));
            writer.Flush();
        }
    }
}
