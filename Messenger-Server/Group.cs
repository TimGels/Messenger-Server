using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public class Group
    {
        private List<Client> clients;
        private Object clientsLock = new Object();
        public String Name { get; set; }
        public int GroupID { get; set; }

        public Group(String name, int id)
        {
            this.Name = name;
            this.GroupID = id;
        }

        public void AddClient(Client client)
        {
            lock (clientsLock)
            {
                this.clients.Add(client);
            }
        }

        public void RemoveClient(Client client)
        {
            lock (clientsLock)
            {
                this.clients.Remove(client);
            }
        }

        public void SendMessageToClients(Message message)
        {
            foreach (Client client in this.clients)
            {
                if (client != message.Sender)
                {
                    Task.Run(() => client.SendData(message));
                }
                
            }
        }
    }
}
