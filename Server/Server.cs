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
        /// <summary>
        /// Port number to listen on.
        /// </summary>
        private readonly int port = 5000; //TODO: make configurable?

        /// <summary>
        /// The listener which listens for new connections
        /// </summary>
        private TcpListener server;

        /// <summary>
        /// All registered groups.
        /// </summary>
        private readonly List<Group> groups;

        /// <summary>
        /// The read-write lock for the grouplist.
        /// </summary>
        private readonly ReaderWriterLockSlim groupLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Hold the lazy-initialized server instance.
        /// </summary>
        private static readonly Lazy<Server> lazy = new Lazy<Server>(() => new Server());

        public static void Main(string[] args)
        {
            Server.Instance.StartListening();
        }

        /// <summary>
        /// Server singleton instance.
        /// </summary>
        public static Server Instance
        {
            get { return lazy.Value; }
        }

        private Server()
        {
            this.groups = new List<Group>();
        }

        /// <summary>
        /// Continuously listen for incoming client connections. Upon a new
        /// connection, hand the associated client to a dedicated listener
        /// thread on the threadpool.
        /// </summary>
        public void StartListening()
        {
            try
            {
                // Listen on both loopback and normal network adapters.
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

        /// <summary>
        /// Retrieve a group by its GroupID.
        /// </summary>
        /// <param name="Id">The GroupID</param>
        /// <returns>The group with the associated ID.</returns>
        public Group GetGroup(int Id)
        {
            groupLocker.EnterReadLock();

            try
            {
                return groups.Where(group => group.Id == Id).FirstOrDefault();
            }
            finally
            {
                groupLocker.ExitReadLock();
            }
        }

        /// <summary>
        /// Create a new group with the specified name.
        /// TODO: Make GroupID unique.
        /// </summary>
        /// <param name="groupName">The requested name of the group.</param>
        /// <returns>The group that was added.</returns>
        public Group CreateGroup(string groupName)
        {
            // Lock here since we need the group count for the GroupID.
            groupLocker.EnterWriteLock();
            try
            {
                Group group = new Group(groupName, this.groups.Count);
                this.groups.Add(group);
                return group;
            }
            finally
            {
                groupLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// This method is executed on the threadpool and calls the clients' ReadData()
        /// method which keeps listening for new messages from the client.
        /// </summary>
        /// <param name="obj">TcpClient object which is passed as a generic object.</param>
        public void DoClientWork(object obj)
        {
            Client client = new Client(obj as TcpClient, -1, null);
            client.ReadData();
        }
    }
}
