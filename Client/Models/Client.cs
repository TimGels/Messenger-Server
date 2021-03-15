using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UWP_Messenger_Client.Models;

namespace UWP_Messenger_Client
{
    public sealed class Client
    {
        /// <summary>
        /// Hold the lazy-initialized client instance.
        /// </summary>
        private static readonly Lazy<Client> lazy = new Lazy<Client>(() => new Client());

        /// <summary>
        /// Port number to send to.
        /// </summary>
        private readonly int port = 5000; //TODO: make configurable?

        /// <summary>
        /// ipAdress of the server.
        /// TODO: make configurable.
        /// TODO: save ip in the appropiate ipaddress class?
        /// </summary>
        private readonly string serverAddress = "127.0.0.1";

        /// <summary>
        /// The read-write lock for the grouplist.
        /// </summary>
        private readonly ReaderWriterLockSlim groupLocker = new ReaderWriterLockSlim();

        public static Client Instance { get { return lazy.Value; } }
        public string ClientName { get; set; }
        public int Id { get; set; }
        public List<Group> Groups { get; set; }
        public Connection Connection { get; set; }


        private Client()
        {
            this.Groups = new List<Group>();
            this.Connection = new Connection(serverAddress, port);
        }
        /// <summary>
        /// TODO: Make GroupID unique.
        /// </summary>
        /// <param name="group"></param>

        public void AddGroup(Group group)
        {
            // Lock here since we need the group count for the GroupID.
            groupLocker.EnterWriteLock();
            try
            {
                this.Groups.Add(group);
            }
            finally
            {
                groupLocker.ExitWriteLock();
            }
        }

        public Group GetGroup(int id)
        {
            groupLocker.EnterReadLock();

            try
            {
                return Groups.Where(group => group.Id == id).FirstOrDefault();
            }
            finally
            {
                groupLocker.ExitReadLock();
            }
        }
        /// <summary>
        /// implement this method with async io oprations.
        /// </summary>
        public void ExportMessageToFile()
        {
            throw new NotImplementedException();
        }

    }
}
