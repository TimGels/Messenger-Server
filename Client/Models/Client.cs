using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared;
using Messenger_Client.Models;
using Windows.Storage.Pickers;
using Connection = Messenger_Client.Models.Connection;
using Group = Messenger_Client.Models.Group;

namespace Messenger_Client
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
            this.ClientName = "clientName";
            this.Id = 1;
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
        /// This method opens a file picker screen. In this way a client can choose where to store the csv
        /// then it will loop throug all groups to get a csv string of all messages in that group.
        /// </summary>
        public async Task ExportMessageToFileAsync()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("CSV", new List<string>() { ".csv" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            //enter read lock of groups
            this.groupLocker.EnterReadLock();

            string csvString = "";
            try
            {
                foreach (Group group in this.Groups)
                {
                    //get csv string of all messages in the group
                    csvString += group.GetMessageCsv();
                }
            }
            finally
            {
                //exit readlock of groups
                this.groupLocker.ExitReadLock();
            }

            await Windows.Storage.FileIO.WriteTextAsync(file, csvString);
        }

    }
}
