using MessengerServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UWP_Messenger_Server;

namespace Messenger_Server
{
    public sealed class Server
    {
        private int port = 5000;
        private TcpListener server;
        private List<Client> clients;
        private List<Group> groups;
        private object clientsLocker = new object();
        private object groupsLocker = new object();
        private static readonly Lazy<Server> lazy = new Lazy<Server>(() => new Server());

        public static void Main(string[] args)
        {
            Server server = Server.Instance;
            server.StartListening();

        }

        public static Server Instance
        {
            get { return lazy.Value; }
        }

        private Server()
        {
            this.clients = new List<Client>();
            this.groups = new List<Group>();
        }

        public void StartListening()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                Console.WriteLine("Waiting for incoming connections... ");

                // Enter the listening loop.
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("New connection!");
                    ThreadPool.QueueUserWorkItem(DoClientWork, client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public void DoClientWork(object obj)
        {
            //construct new client
            Client client = new Client(obj as TcpClient);
            lock (clientsLocker)
            {
                this.clients.Add(client);
            }
            //call the read data loop.
            client.ReadData();
        }
    }
}
