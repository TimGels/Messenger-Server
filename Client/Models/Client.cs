using Messenger_Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using Messenger_Client.Views;

namespace Messenger_Client
{
    public sealed class Client
    {
        /// <summary>
        /// Hold the lazy-initialized client instance.
        /// </summary>
        private static readonly Lazy<Client> lazy = new Lazy<Client>(() => new Client());

        /// <summary>
        /// The read-write lock for the grouplist.
        /// </summary>
        private readonly ReaderWriterLockSlim groupLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Singleton Instance.
        /// </summary>
        public static Client Instance { get { return lazy.Value; } }

        /// <summary>
        /// ID for this client. Is given by the server.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of this client. Is saved in the server and retrieved on logon.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This collection is used to store all the groups.
        /// It's observable because the front-end binds to this property.
        /// </summary>
        public ObservableCollection<Group> Groups { get; set; }

        /// <summary>
        /// Object which holds all the Connection logic.
        /// </summary>
        public Connection Connection { get; set; }

        /// <summary>
        /// Default constructor for this client.
        /// It's private because this class is a Singleton.
        /// </summary>
        private Client()
        {
            this.Groups = new ObservableCollection<Group>();
            this.Connection = new Connection();

            //Default name. Is overwritten on logon.
            this.Name = "ClientNameNeedsToBeSet";
        }

        /// <summary>
        /// Function to find out if plinq is enabled.
        /// </summary>
        /// <returns></returns>
        public static bool IsPlinqEnabled()
        {
            _ = bool.TryParse(ApplicationData.Current.LocalSettings.Values["UsePLINQ"].ToString(), out bool plinqEnabled);
            return plinqEnabled;
        }

        /// <summary>
        /// Add a group to the client.
        /// </summary>
        /// <param name="group"></param>
        public void AddGroup(Group group)
        {
            // Lock here since we need the group count for the GroupID.
            groupLocker.EnterWriteLock();
            try
            {
                Helper.RunOnUI(() => this.Groups.Add(group));
            }
            finally
            {
                groupLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Function for getting a group by it's id.
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The group with the given ID or null if not found</returns>
        public Group GetGroup(int id)
        {
            groupLocker.EnterReadLock();

            try
            {
                if (!IsPlinqEnabled())
                {
                    return Groups.Where(group => group.Id == id).FirstOrDefault();
                }
                else
                {
                    return Groups.AsParallel().Where(group => group.Id == id).FirstOrDefault();
                }
            }
            finally
            {
                groupLocker.ExitReadLock();
            }
        }

        /// <summary>
        /// Function for removing the given group.
        /// </summary>
        /// <param name="group"></param>
        public void RemoveGroup(Group group)
        {
            groupLocker.EnterWriteLock();
            try
            {
                this.Groups.Remove(group);
            }
            finally
            {
                groupLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// This method opens a file picker screen. In this way a client can choose where to store the csv
        /// then it will loop throug all groups to get a csv string of all messages in that group.
        /// </summary>
        public async Task ExportMessageToFileAsync()
        {
            FileSavePicker savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "New Document"
            };

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("CSV", new List<string>() { ".csv" });

            StorageFile file = await savePicker.PickSaveFileAsync();

            if (file is null)
            {
                return;
            }

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

            await FileIO.WriteTextAsync(file, csvString);
        }

        /// <summary>
        /// Logout function which closes the connection, clears the groups and navigates to the LoginPage.
        /// </summary>
        public void Logout()
        {
            this.Connection.Close();
            groupLocker.EnterWriteLock();
            try
            {
                this.Groups.Clear();
            }
            finally
            {
                groupLocker.ExitWriteLock();
            }

            Helper.NavigateTo(typeof(LoginPage));
        }
    }
}
