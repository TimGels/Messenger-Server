using Shared;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public class Connection : Shared.Connection
    {
        public Connection(TcpClient client)
            : base(client)
        {
            
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
                        Task.Run(() => CommunicationHandler.HandleMessage(this, new Message(data)));
                    }
                }
                catch (Exception e)
                {
                    // Upon exception, most likely disconnect has occured,
                    // so delete connection from dictionary.
                    Console.WriteLine(e.GetType().FullName);
                    Server.Instance.DeleteConnection(this);
                    break;
                }
            }
        }
    }
}
