using Shared;
using System.Collections.ObjectModel;
using System.Threading;

namespace Messenger_Client.Models
{
    public class Group : Shared.Group
    {
        /// <summary>
        /// Holds all the message that are sended.
        /// It's an observable collection because the front end binds to it.
        /// </summary>
        public ObservableCollection<Message> Messages { get; set; }

        /// <summary>
        /// The read-write lock for the messages list.
        /// </summary>
        private readonly ReaderWriterLockSlim messsageLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Create a new group from a base group.
        /// </summary>
        /// <param name="group">The base group</param>
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
                // Uses the dispather because the list in the viewmodel is bound to the
                // message list in group. Modifying the list without the dispatcher will
                // throw a COMException (RPC_E_WRONG_THREAD).
                Helper.RunOnUI(() => this.Messages.Add(message));
            }
            finally
            {
                this.messsageLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// returns a csv string wich respresents all the message in the group.
        /// </summary>
        /// <returns>The generated csv</returns>
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
