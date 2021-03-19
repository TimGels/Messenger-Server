using Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger_Client.Models
{
    public class Group
    {
        public List<Message> Messages { get; set; }
        public String Name { get; set; }
        public int Id { get; set; }
        /// <summary>
        /// The read-write lock for the messages list.
        /// </summary>
        private readonly ReaderWriterLockSlim messsageLocker = new ReaderWriterLockSlim();

        public Group(String name, int id)
        {
            this.Id = id;
            this.Name = name;
            this.Messages = new List<Message>();
        }

        /// <summary>
        /// TODO: is this method neccesary if the Messages property is public? 
        /// This method should be used by the communcation handler. 
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(Message message)
        {
            this.messsageLocker.EnterWriteLock();
            try
            {
                this.Messages.Add(message);
            }
            finally
            {
                this.messsageLocker.ExitWriteLock();
            }
            
        }

        public void SendMessage(String payload)
        {
            //TODO: I think this method should be responsible for constructing a new message OR for adding the id and the name of the group to the message.
            // from here a call to the connection object is made which actually sends the message to the server.

            //create message and set the payload.
            //add group id and name?
            Message message = new Message()
            {
                GroupID = this.Id,
                ClientId = Client.Instance.Id,
                ClientName = Client.Instance.ClientName,
                PayloadData = payload,
                PayloadType = "text",
                MessageType = MessageType.ChatMessage
            };
            //call send method which actually sends this message to the server.
            Task.Run(()=> Client.Instance.Connection.SendMessage(message));

        }
    }
}
