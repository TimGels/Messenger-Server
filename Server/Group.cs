using Shared;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public class Group : Shared.Group
    {
        /// <summary>
        /// List with all clients currently registered in the group.
        /// </summary>
        private readonly List<Client> clients;

        /// <summary>
        /// Lock for reading from and writing to the clients list.
        /// </summary>
        private readonly ReaderWriterLockSlim clientsLock = new ReaderWriterLockSlim();

        public Group(int id, string name)
            : base(id, name)
        {
            this.clients = new List<Client>();
        }

        /// <summary>
        /// Finds out if a client with the specified Id exists in this group.
        /// </summary>
        /// <param name="clientId">The Id to search for.</param>
        /// <returns>True if client with specified Id exists in group, otherwise false.</returns>
        public bool ContainsClient(int clientId)
        {
            clientsLock.EnterReadLock();

            try
            {
                return clients.Find(client => client.Id == clientId) != null;
            }
            finally
            {
                clientsLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Add a new client to the group. This method is thread-safe.
        /// </summary>
        /// <param name="client">The client to add.</param>
        public void AddClient(Client client)
        {

            clientsLock.EnterWriteLock();
            try
            {
                this.clients.Add(client);
            }
            finally
            {
                clientsLock.ExitWriteLock();
            }

        }

        /// <summary>
        /// Remove a client from the group. This method is thread-safe.
        /// </summary>
        /// <param name="client">The client to remove.</param>
        public void RemoveClient(Client client)
        {
            clientsLock.EnterWriteLock();
            try
            {
                this.clients.Remove(client);
                DatabaseHandler.DeleteGroupParticipant(this.Id, client.Id);
            }
            finally
            {
                clientsLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Send the specified message to all clients in this group.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToClients(Message message)
        {
            // TODO: Add message to this group in the database.

            // Lock the client list for reading, start sending messages and wait
            // for all tasks to complete before exiting the read lock.
            List<Task> sendDataTasks = new List<Task>();
            clientsLock.EnterReadLock();
            foreach (Client client in this.clients)
            {
                // Don't return the message to the original sender.
                if (client.Id != message.ClientId)
                {
                    sendDataTasks.Add(Task.Run(() =>
                    {
                        Connection connection = Server.Instance.GetConnection(client.Id);
                        if (connection != null)
                        {
                            connection.SendData(message);
                        }
                    }));
                }
            }
            Task.WaitAll(sendDataTasks.ToArray());
            clientsLock.ExitReadLock();
        }
    }
}
