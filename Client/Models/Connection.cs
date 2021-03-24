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
    public class Connection : Shared.Connection
    {
        public Connection(string serverAddress, int port)
            : base(new TcpClient(serverAddress, port))
        {
            //start readerThread
            new Thread(ReadData).Start();
        }

        public override void ReadData()
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
    }
}
