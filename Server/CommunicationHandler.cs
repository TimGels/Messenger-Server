using Shared;
using System.Linq;

namespace Messenger_Server
{
    public static class CommunicationHandler
    {
        /// <summary>
        /// This method is called by the ReadData method in Client.
        /// It handles all incoming messages from clients.
        /// </summary>
        /// <param name="connection">The connection from which the message was send.</param>
        /// <param name="message">The incoming message.</param>
        public static void HandleMessage(Connection connection, Message message)
        {
            switch (message.MessageType)
            {
                case MessageType.RegisterClient:
                    HandleRegisterClient(connection, message);
                    break;
                case MessageType.SignInClient:
                    HandleSignInClient(connection, message);
                    break;
                case MessageType.SignOutClient:
                    HandleSignOutClient(connection);
                    break;
                case MessageType.RegisterGroup:
                    HandleRegisterGroup(connection, message);
                    break;
                case MessageType.RequestGroups:
                    HandleRequestGroups(connection);
                    break;
                case MessageType.JoinGroup:
                    HandleJoinGroup(connection, message);
                    break;
                case MessageType.LeaveGroup:
                    HandleLeaveGroup(connection, message);
                    break;
                case MessageType.ChatMessage:
                    // Relay the chatMessage to all other clients in the group.
                    //Server.Instance.GetGroup(message.GroupID).SendMessageToClients(message);
                    break;
            }
        }

        /// <summary>
        /// Handle incoming registration requests. Query database to verify and add the
        /// incoming data. When succesful, send the new client id. If registration is not
        /// possible, client id is -1.
        /// </summary>
        /// <param name="connection">The connection from which the request was send.</param>
        /// <param name="message">The incoming registration message.</param>
        private static void HandleRegisterClient(Connection connection, Message message)
        {
            // TODO: Add database validation and unsuccesful response.
            int retreivedid = 0;
            Client newClient = new Client()
            {
                Id = retreivedid,
                Name = message.ClientName
            };

            // add client in database / internal dictionary
            Server.Instance.AddClient(newClient, connection);

            connection.SendData(new Message()
            {
                MessageType = MessageType.RegisterClientResponse,
                ClientId = newClient.Id
            });
        }

        /// <summary>
        /// Handles incoming signin requests. Validate the incoming data with the database
        /// and send a response based upon the result.
        /// </summary>
        /// <param name="connection">The connection from which the request was sent.</param>
        /// <param name="message">The incoming signin message.</param>
        private static void HandleSignInClient(Connection connection, Message message)
        {
            // TODO: Add database validation and unsuccesful response.
            int databaseclientid = 99;
            string databaseusername = "name";

            Server.Instance.AddConnection(databaseclientid, connection);

            connection.SendData(new Message()
            {
                MessageType = MessageType.SignInClientResponse,
                ClientId = databaseclientid,
                ClientName = databaseusername
            });
        }

        /// <summary>
        /// Handle incoming signout requests. Upon a signout request, close the connection.
        /// </summary>
        /// <param name="connection">The connection from which the request was sent.</param>
        private static void HandleSignOutClient(Connection connection)
        {
            connection.Close();
        }

        private static void HandleRegisterGroup(Connection connection, Message message)
        {
            // Create a new group, add the sender as initial group member
            // and return the ID of the new group.
            Group newGroup = Server.Instance.CreateGroup(message.PayloadData);
            //newGroup.AddClient(client);
            Message response = new Message()
            {
                GroupID = newGroup.Id,
                MessageType = MessageType.RegisterGroupResponse
            };
            connection.SendData(response);
        }

        /// <summary>
        /// Handle incoming group list requests. The server returns a list of groups
        /// encoded as a string with Id's and names together.
        /// </summary>
        /// <param name="connection"></param>
        private static void HandleRequestGroups(Connection connection)
        {
            connection.SendData(new Message()
            {
                MessageType = MessageType.RequestGroupsResponse,
                GroupList = Server.Instance.Groups.Cast<Shared.Group>().ToList()
            });
        }

        private static void HandleJoinGroup(Connection connection, Message message)
        {

        }

        private static void HandleLeaveGroup(Connection connection, Message message)
        {

        }
    }
}
