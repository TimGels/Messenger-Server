using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private List<Group> groups;

        /// <summary>
        /// The read-write lock for the grouplist.
        /// </summary>
        private readonly ReaderWriterLockSlim groupLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Provides a link between clients and connections. All keys (clients) represent
        /// the registered clients. If the value (connection) is non-null, the client has
        /// logged in.
        /// </summary>
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
            AddClientsToGroups();
        }

        /// <summary>
        /// This method adds all the clients to the groups based on the database.
        /// </summary>
        private void AddClientsToGroups()
        {
            //DatabaseHandler.getGroupsParticipants() returns a dictionary with: groupId, userId;
            foreach (KeyValuePair<int, int> entry in DatabaseHandler.GetGroupParticipants())
            {
                int groupId = entry.Key;
                int userId = entry.Value;

                //get corresponding group object
                Group group = this.GetGroup(groupId);

                //get corresponding client object
                Client client = this.GetClient(userId);

                //check if client and group are not null
                if (client != null && group != null)
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
            int groupID = DatabaseHandler.AddGroup(groupName);
            Group group = new Group(groupID, groupName);


            groupLocker.EnterWriteLock();
            try
            {
                this.groups.Add(group);
                return group;
            }
            finally
            {
                groupLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Add a client to the server
        /// </summary>
        /// <param name="client"></param>
        /// <returns>The id of the just created client</returns>
        public int CreateAndAddClient(string userName, string email, string password)
        {
            Client client = new Client()
            {
                Email = email,
                Name = userName
            };

            client.Id = DatabaseHandler.AddClient(client, password);

            if (client.Id >= 0)
            {
                clientLocker.EnterWriteLock();

                try
                {
                    this.clients.Add(client, null);
                }
                finally
                {
                    clientLocker.ExitWriteLock();
                }
            }
            return client.Id;
        }

        /// <summary>
        /// Get a client by its Id.
        /// </summary>
        /// <param name="id">The Id of the client to get.</param>
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
        /// Get a client by its email.
        /// </summary>
        /// <param name="email">The email of the client to get.</param>
        /// <returns></returns>
        public Client GetClient(string email)
        {
            clientLocker.EnterReadLock();

            try
            {
                return clients.Keys.Where(client => client.Email == email).FirstOrDefault();
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
                var keyValuePairs = clients.Where(pair => connection.Equals(pair.Value));

                // Shouldn't happen, checking just in case.
                if (keyValuePairs == null)
                {
                    return;
                }
                // FirstOrDefault throws if keyValuePairs is null.
                Client client = keyValuePairs.FirstOrDefault().Key;
                if (client != null)
                {
                    // Remove connection for found client.
                    clients[client] = null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
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
        /// <returns>The connection linked tot the client, if found. Null otherwise.</returns>
        public Connection GetConnection(int id)
        {
            clientLocker.EnterReadLock();

            try
            {
                Client client = clients.Keys.Where(client => client.Id == id).FirstOrDefault();

                if (client == null)
                {
                    return null;
                }
                else
                {
                    return clients[client];
                }
            }
            finally
            {
                clientLocker.ExitReadLock();
            }
        }

        /// <summary>
        /// getter for the group list
        /// </summary>
        /// <returns>a copy of the groups list in the server</returns>
        public List<Group> GetGroups()
        {
            groupLocker.EnterReadLock();
            try
            {
                return new List<Group>(this.groups);
            }
            finally
            {
                groupLocker.ExitReadLock();
            }

        }

        public List<Group> getGroupsWithClient(Client client)
        {
            List<Group> groupsWithClient = new List<Group>();
            groupLocker.EnterReadLock();
            try
            {
                foreach (Group group in this.groups)
                {
                    if (group.ContainsClient(client.Id))
                    {
                        groupsWithClient.Add(group);
                    }
                }
            }
            finally
            {
                groupLocker.ExitReadLock();
            }

            return groupsWithClient;
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
