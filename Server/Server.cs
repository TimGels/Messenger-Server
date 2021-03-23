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

        private readonly Dictionary<Client, Connection> clients;

        private readonly ReaderWriterLockSlim clientLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Hold the lazy-initialized server instance.
        /// </summary>
        private static readonly Lazy<Server> lazy = new Lazy<Server>(() => new Server());

        public static void Main(string[] args)
        {
            DatabaseHandler.CreateDatabase();
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
            this.groups = DatabaseHandler.GetGroups();
            this.clients = DatabaseHandler.GetClients();
            addClientsToGroups();
        }

        /// <summary>
        /// This method adds all the clients to the groups based on the database.
        /// </summary>
        private void addClientsToGroups()
        {
            //DatabaseHandler.getGroupsParticipants() returns a dictionary with: groupId, userId;
            foreach(KeyValuePair<int, int> entry in DatabaseHandler.GetGroupParticipants())
            {
                int groupId = entry.Key;
                int userId = entry.Value;

                //get corresponding group object
                Group group = this.GetGroup(groupId);

                //get corresponding client object
                Client client = this.GetClient(userId);

                //check if client and group are not null
                if(client != null && group != null)
                {
                    group.AddClient(client);
                }
            }
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
        /// <param name="id">The GroupID</param>
        /// <returns>The group with the associated ID.</returns>
        public Group GetGroup(int id)
        {
            groupLocker.EnterReadLock();

            try
            {
                return groups.Where(group => group.Id == id).FirstOrDefault();
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
        /// Add a client to the server with its corresponding connection.
        /// </summary>
        /// <param name="client">The client to add.</param>
        /// <param name="connection">The connection to add.</param>
        public void AddClient(Client client, Connection connection)
        {
            clientLocker.EnterWriteLock();

            try
            {
                clients.Add(client, connection);
            }
            finally
            {
                clientLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Get a client by its Id.
        /// </summary>
        /// <param name="clientId">The Id of the client to get.</param>
        /// <returns></returns>
        public Client GetClient(int id)
        {
            clientLocker.EnterReadLock();

            try
            {
                return clients.Keys.Where(client => client.Id == id).FirstOrDefault();
            }
            finally
            {
                clientLocker.ExitReadLock();
            }
        }

        /// <summary>
        /// Adds a connection to an existing client.
        /// </summary>
        /// <param name="id">The Id of the existing client.</param>
        /// <param name="connection">The connection to add.</param>
        public void AddConnection(int id, Connection connection)
        {
            clientLocker.EnterWriteLock();

            try
            {
                // Don't call GetClient which enters read lock, since we already hold write lock.
                Client client = clients.Keys.Where(client => client.Id == id).FirstOrDefault();
                clients[client] = connection;
            }
            finally
            {
                clientLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Delete the specified connection from the client/connection dictionary.
        /// </summary>
        /// <param name="connection">The connection to remove.</param>
        public void DeleteConnection(Connection connection)
        {
            Console.WriteLine("Removing connection!");

            clientLocker.EnterWriteLock();

            try
            {
                // HACK: Only works when values (connections) in Server.clients are unique

                // Find client with the specified connection which we need to delete.
                Client client = clients.Where(pair => pair.Value.Equals(connection)).FirstOrDefault().Key;

                // Remove connection for found client.
                clients[client] = null;
            }
            finally
            {
                clientLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Retrieve a connection based on a client id.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <returns></returns>
        public Connection GetConnection(int id)
        {
            clientLocker.EnterReadLock();

            try
            {
                Client client = clients.Keys.Where(client => client.Id == id).FirstOrDefault();
                return clients[client];
            }
            finally
            {
                clientLocker.ExitReadLock();
            }
        }

        /// <summary>
        /// This method is executed on the threadpool and calls the clients' ReadData()
        /// method which keeps listening for new messages from the client.
        /// </summary>
        /// <param name="obj">TcpClient object which is passed as a generic object.</param>
        public void DoClientWork(object obj)
        {
            new Connection(obj as TcpClient).ReadData();
        }
    }
}
