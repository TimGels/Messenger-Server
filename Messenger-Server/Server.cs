using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Messenger_Server
{
    public sealed class Server
    {
        private int port = 5000;
        private TcpListener server;
        private List<Client> clients;
        private List<Group> groups;
        private object clientsLocker = new object();
        private static readonly Lazy<Server> lazy = new Lazy<Server>(() => new Server());
        ReaderWriterLockSlim groupLocker = new ReaderWriterLockSlim();


        public static void Main(string[] args)
        {
            Server.Instance.StartListening();
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

        public Group GetGroup(int Id)
        {
            groupLocker.EnterReadLock();

            try
            {
                return groups.Where(group => group.GroupID == Id).FirstOrDefault();
            }
            finally
            {
                groupLocker.ExitReadLock();
            }
        }

        public int CreateGroup(String groupName)
        {
            groupLocker.EnterWriteLock();
            try
            {
                Group g = new Group(groupName, this.groups.Count);
                this.groups.Add(g);
                return g.GroupID;
            }
            finally
            {
                groupLocker.ExitWriteLock();
            }
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
