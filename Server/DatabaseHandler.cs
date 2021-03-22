using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger_Server
{
    static class DatabaseHandler
    {
        private static SqliteConnection connection = createConnection();

        private static SqliteConnection createConnection()
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = Path.Combine(Directory.GetCurrentDirectory(), "messenger.db");
            return new SqliteConnection(connectionStringBuilder.ToString());
        }

        public static int addClient(Client client)
        {
            //SqliteCommand command = new SqliteCommand("insert into `User` (userName, email, passwd) values(@username, @email, @password)", connection);
            SqliteCommand command = new SqliteCommand("insert into `User` (userName, passwd) values(@username, @password)", connection);
            command.Parameters.AddWithValue("@username", client.Name);
            //command.Parameters.AddWithValue("@email", client.Email);
            command.Parameters.AddWithValue("@password", "bigSecretThing");

            SqliteCommand getIdCommand = new SqliteCommand("select last_insert_rowid() from `User`", connection);

            connection.Open();
            command.ExecuteNonQuery();
            int id = (int)(long)getIdCommand.ExecuteScalar();

            return id;
        }
    }
}
