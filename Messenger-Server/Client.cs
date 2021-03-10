using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

        public void SendData(Message message)
        {
            writer.WriteLine(Message.SerializeMessage(message));
            writer.Flush();
        }
    }
}
