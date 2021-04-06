using Microsoft.Data.Sqlite;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Messenger_Server
{
    public static class DatabaseHandler
    {
        /// <summary>
        /// The database file is always stored in the same folder as the executable.
        /// </summary>
        private static readonly string databaseFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "messenger.db");

        private static readonly string connectionString = CreateConnectionString();

        public static void Initialize()
        {
            CreateDatabase();
            SetJournalModeDatabase();
        }

        private static string CreateConnectionString()
        {
            // Create new connection string builder and set the dataSource to the databaseFilePath.
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = databaseFilePath
            };

            if (IsDatabaseSharedCacheEnabled())
            {
                Console.WriteLine("Datbaseconnections will be created with Shared Cache!");
                connectionStringBuilder.Cache = SqliteCacheMode.Shared;
            }

            return connectionStringBuilder.ToString();
        }

        private static void SetJournalModeDatabase()
        {
            string walEnabledSetting = Configuration.GetSetting("walEnabled");
            bool enableWal = false;

            if (walEnabledSetting != null)
            {
                if (bool.Parse(walEnabledSetting))
                {
                    enableWal = true;
                }
            }

            ExecuteJournalModeQuery(enableWal);
        }

        private static bool IsDatabaseSharedCacheEnabled()
        {
            string valueFromSetting = Configuration.GetSetting("databaseCacheShared");
            if(valueFromSetting == null)
            {
                return false;
            } else
            {
                return bool.Parse(valueFromSetting);
            }
        }

        private static void ExecuteJournalModeQuery(bool enabled)
        {
            string status = "";
            if (enabled && !IsDatabaseSharedCacheEnabled())
            {
                status = "Shared Cache is not enabled! WAL isn't possible.\n";
                enabled = false;
            }
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                // Enable or disable write-ahead logging
                SqliteCommand command = connection.CreateCommand();
                if (enabled)
                {
                    command.CommandText = @"PRAGMA journal_mode = 'wal'";
                } else
                {
                    command.CommandText = @"PRAGMA journal_mode = 'delete'";
                }
                
                connection.Open();
                status += "Database journal mode was set to: " + (string)command.ExecuteScalar();
            }

            Console.WriteLine(status);
        }

        /// <summary>
        /// Method for creating the database. This method will call several createTable methods.
        /// When calling this methods, the database file will be created if it not exists.
        /// </summary>
        private static void CreateDatabase()
        {
            // If the database file already exists, we assume that the neccessary tables are already properly created.
            if (File.Exists(databaseFilePath))
            {
                return;
            }
            // The database file is automatically created by calling the Open method on connection.
            // Therefore the non-existing file will be created in the first create table method.
            CreateUserTable();
            CreateGroupTable();
            CreateGroupParticipantsTable();
            CreateMessageTable();

        }

        /// <summary>
        /// This method will create the message table.
        /// </summary>
        /// <param name="connectionStringBuilder"></param>
        private static void CreateMessageTable()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE `Message` (id INTEGER PRIMARY KEY,payload TEXT, messageType VARCHAR(30), senderid INTEGER,groupid INTEGER,timesent DATETIME,FOREIGN KEY(senderid) REFERENCES `User`(id) ON DELETE CASCADE FOREIGN KEY(groupid) REFERENCES `Group`(id) ON DELETE CASCADE)";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// This method will create the GroupParticipants table.
        /// </summary>
        private static void CreateGroupParticipantsTable()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE `GroupParticipants` (userid INTEGER NOT NULL,groupid INTEGER NOT NULL,FOREIGN KEY(userid) REFERENCES `User`(id) ON DELETE CASCADE FOREIGN KEY(groupid) REFERENCES `Group`(id) ON DELETE CASCADE PRIMARY KEY(userid, groupid))";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// This method will create the group table.
        /// </summary>
        private static void CreateGroupTable()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE `Group` (id INTEGER PRIMARY KEY,groupName VARCHAR(50))";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// This method will create the user table.
        /// </summary>
        private static void CreateUserTable()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                SqliteCommand command = connection.CreateCommand();
                connection.Open();
                command.CommandText = "CREATE TABLE `User` (id INTEGER PRIMARY KEY,userName VARCHAR(50),email VARCHAR(100) UNIQUE,passwd VARCHAR(100))";
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// Insert a message into the database.
        /// </summary>
        /// <param name="message">The message to add.</param>
        public static void AddMessage(Message message)
        {
            try
            {
                using(SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    SqliteCommand command = connection.CreateCommand();
                    command.CommandText = "insert into `Message` (payload, messageType, senderid, groupid, timesent) values (@payload, @messageType, @senderid, @groupid, @timesent)";
                    command.Parameters.AddWithValue("@payload", message.PayloadData);
                    command.Parameters.AddWithValue("@messageType", message.MessageType);
                    command.Parameters.AddWithValue("@senderid", message.ClientId);
                    command.Parameters.AddWithValue("@groupid", message.GroupID);
                    command.Parameters.AddWithValue("@timesent", message.DateTime);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Add the given client to the database. Then it will set and return the id of
        /// the client.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="password"></param>
        /// <returns>int < 0 means there already exists a client with this email or something
        /// else went wrong, int >= 0 means id of the just added user.</returns>
        public static int AddClient(Client client, string password)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    SqliteCommand command = connection.CreateCommand();
                    command.CommandText = "insert into `User` (userName, email, passwd) values (@username, @email, @password)";
                    command.Parameters.AddWithValue("@username", client.Name);
                    command.Parameters.AddWithValue("@email", client.Email);
                    command.Parameters.AddWithValue("@password", password);

                    SqliteCommand getIdCommand = new SqliteCommand("select last_insert_rowid() from `User`", connection);

                    connection.Open();
                    command.ExecuteNonQuery();
                    int id = (int)(long)getIdCommand.ExecuteScalar();
                    client.Id = id;
                    return id;
                }
            }
            catch (Exception) //TODO: create better catch for uniqueness error!!
            {
                return -1;
            }
        }

        /// <summary>
        /// This method will add the given group to the database. 
        /// </summary>
        /// <param name="groupName">The name of the group to add.</param>
        /// <returns>-1 when somethin went wrong, else the id of added group.</returns>
        public static int AddGroup(string groupName)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    SqliteCommand command = connection.CreateCommand();
                    command.CommandText = "insert into `Group` (groupName) values (@groupName)";
                    command.Parameters.AddWithValue("@groupName", groupName);

                    SqliteCommand getIdCommand = new SqliteCommand("select last_insert_rowid() from `Group`", connection);

                    connection.Open();
                    command.ExecuteNonQuery();
                    return (int)(long)getIdCommand.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        /// <summary>
        /// This method will add the group client relation to the database.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="client"></param>
        /// <returns>1 for succes, -1 when something went wrong</returns>
        public static int AddClientToGroup(Group group, Client client)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    SqliteCommand command = connection.CreateCommand();
                    command.CommandText = "insert into `GroupParticipants` (userid, groupid) values (@userId, @groupId)";
                    command.Parameters.AddWithValue("@userId", client.Id);
                    command.Parameters.AddWithValue("@groupId", group.Id);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        /// <summary>
        /// Get the password from the client based on the email of the client.
        /// </summary>
        /// <param name="email">The email of the client.</param>
        /// <returns>Password for the given email, null if user with email does not exist.</returns>
        public static string GetPasswordFromClient(string email)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    SqliteCommand command = connection.CreateCommand();
                    command.CommandText = "select passwd from `User` where email = @userEmail";
                    command.Parameters.AddWithValue("@userEmail", email);

                    connection.Open();
                    return (string)command.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a list with all the groups in the database.
        /// </summary>
        /// <returns></returns>
        public static List<Group> GetGroups()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT id, groupName from `Group`;";
                List<Group> groups = new List<Group>();

                connection.Open();
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Group group = new Group(reader.GetInt32(0), reader.GetString(1));
                        groups.Add(group);
                    }
                }
                return groups;
            }
        }

        /// <summary>
        /// Get all clients. purpose: load all client in database to running server.
        /// </summary>
        /// <returns>A dictionary with all the clients in the database. Connection is null</returns>
        public static Dictionary<Client, Connection> GetClients()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "select id, userName, email FROM `User`;";
                Dictionary<Client, Connection> clients = new Dictionary<Client, Connection>();

                connection.Open();
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Client client = new Client()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Email = reader.GetString(2)

                        };
                        clients.Add(client, null);
                    }
                }
                connection.Close();
                return clients;
            }
        }

        /// <summary>
        /// Gets all the groupParticipants
        /// </summary>
        /// <returns> Dictionary with: GroupID, UserID</returns>
        public static List<KeyValuePair<int, int>> GetGroupParticipants()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT groupid, userid FROM `GroupParticipants`;";
                List<KeyValuePair<int, int>> groupParticipants = new List<KeyValuePair<int, int>>();

                connection.Open();
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groupParticipants.Add(new KeyValuePair<int, int>(reader.GetInt32(0), reader.GetInt32(1)));
                    }
                }
                connection.Close();
                return groupParticipants;
            }
        }

        /// <summary>
        /// Deletes a client from a group in the database
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="userID"></param>
        /// <returns>the number of affected rows</returns>
        public static int DeleteGroupParticipant(int groupID, int userID)
        {
            using(SqliteConnection connection = new SqliteConnection(connectionString))
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "DELETE from `GroupParticipants` WHERE userid == @userid AND groupid == @groupid";
                command.Parameters.AddWithValue("@userid", userID);
                command.Parameters.AddWithValue("@groupid", groupID);

                connection.Open();

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// removes a group from the database.
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns>the number of affected rows</returns>
        public static int RemoveGroup(int groupID)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "DELETE from `Group` WHERE id == @groupid";
                command.Parameters.AddWithValue("@groupid", groupID);

                connection.Open();

                return command.ExecuteNonQuery();
            }
        }
    }
}
