using Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger_Client.Models
{
    public class Group : Shared.Group
    {
        public ObservableCollection<Message> Messages { get; set; }

        /// <summary>
        /// The read-write lock for the messages list.
        /// </summary>
        private readonly ReaderWriterLockSlim messsageLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Create a new group from a base group.
        /// </summary>
        /// <param name="group"></param>
        public Group(Shared.Group group) : this(group.Id, group.Name)
        {

        }

        /// <summary>
        /// Create a new group from an Id and name.
        /// </summary>
        /// <param name="id">The Id of the group.</param>
        /// <param name="name">The name of the group.</param>
        public Group(int id, string name) : base(id, name)
        {
            this.Messages = new ObservableCollection<Message>();
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

        /// <summary>
        /// returns a csv string wich respresents all the message in the group.
        /// </summary>
        /// <returns></returns>
        public string GetMessageCsv()
        {
            //enter read lock of messages
            this.messsageLocker.EnterReadLock();
            string csvMessages = "";
            try
            {
                foreach (Message message in this.Messages)
                {
                    //get the csv representation of each message and add it to the string.
                    csvMessages += message.GetCsvString();
                }

                return csvMessages;
            }
            finally
            {
                //exit readlock of all messages
                this.messsageLocker.ExitReadLock();
            }
        }
    }
}
