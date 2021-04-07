using Messenger_Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.Storage;

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

        public static Client Instance { get { return lazy.Value; } }
        public int Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Group> Groups { get; set; }
        public Connection Connection { get; set; }


        private Client()
        {
            this.Groups = new ObservableCollection<Group>();
            this.Connection = new Connection();
            this.Name = "ClientNameNeedsToBeSet";
        }

        public static bool IsPlinqEnabled()
        {
            _ = bool.TryParse(ApplicationData.Current.LocalSettings.Values["UsePLINQ"].ToString(), out bool plinqEnabled);
            return plinqEnabled;
        }

        /// <summary>
        /// </summary>
        /// <param name="group"></param>
        public void AddGroup(Group group)
        {
            // Lock here since we need the group count for the GroupID.
            groupLocker.EnterWriteLock();
            try
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    this.Groups.Add(group);
                }).AsTask();
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
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("CSV", new List<string>() { ".csv" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

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

            await Windows.Storage.FileIO.WriteTextAsync(file, csvString);
        }

        public void Logout()
        {
            this.Connection.Close();
            groupLocker.EnterWriteLock();
            try
            {
                this.Groups.Clear();
            } finally
            {
                groupLocker.ExitWriteLock();
            }
            
        }
    }
}
