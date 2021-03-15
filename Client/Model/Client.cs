using System;
using System.Collections.Generic;
using UWP_Messenger_Client.Models;

namespace UWP_Messenger_Client
{
    public sealed class Client
    {
        private static readonly Lazy<Client> lazy = new Lazy<Client>(() => new Client());

        public static Client Instance { get { return lazy.Value; } }
        private String clientName;
        private List<Group> group;
        private Connection connection;


        private Client()
        {
        }

        public void CreateGroup(String groupName)
        {
            throw new NotImplementedException();
        }

        public void AddGroup(Group group)
        {
            throw new NotImplementedException();
        }

        public Group GetGroup(String groupName)
        {
            throw new NotImplementedException();
        }

        public Client GetInstance()
        {
            return this;
        }

        public void ExportMessageToFile()
        {
            throw new NotImplementedException();
        }

    }
}
