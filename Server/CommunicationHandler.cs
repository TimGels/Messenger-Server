using Shared;
using System.Linq;
using System;
using System.Collections.Generic;

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
                    HandleRequestGroups(connection, message);
                    break;
                case MessageType.JoinGroup:
                    HandleJoinGroup(connection, message);
                    break;
                case MessageType.LeaveGroup:
                    HandleLeaveGroup(connection, message);
                    break;
                case MessageType.ChatMessage:
                    HandleChatMessage(connection, message);
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
            string email = message.RegisterInfo.Login.Email;
            string password = message.RegisterInfo.Login.Password;
            string userName = message.RegisterInfo.Username;

            string hashedPassword = Helper.HashPassword(password);
            int id = Server.Instance.CreateAndAddClient(userName, email, hashedPassword);

            connection.SendData(new Message(MessageType.RegisterClientResponse)
            {
                ClientId = id
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
            bool authenticated = Helper.ValidatePassword(message.LoginInfo.Email, message.LoginInfo.Password);

            Message response = new Message(MessageType.SignInClientResponse);

            if (!authenticated)
            {
                response.ClientId = -1;
                connection.SendData(response);
                return;
            }

            // Retrieve client by unique email.
            Client client = Server.Instance.GetClient(message.LoginInfo.Email);

            // If the connection already exists, the client is already signed in.
            if (Server.Instance.GetConnection(client.Id) != null)
            {
                response.ClientId = -2;
                connection.SendData(response);
                return;
            }

            // "Sign-in" by adding connection to client.
            Server.Instance.AddConnection(client.Id, connection);

            response.ClientId = client.Id;
            response.ClientName = client.Name;
            response.GroupList = Server.Instance.getGroupsWithClient(client).Cast<Shared.Group>().ToList();

            connection.SendData(response);
        }

        /// <summary>
        /// Handle incoming signout requests. Upon a signout request, close the connection.
        /// </summary>
        /// <param name="connection">The connection from which the request was sent.</param>
        private static void HandleSignOutClient(Connection connection)
        {
            connection.Close();
            //TODO: signoutclienResponse sturen.
        }

        /// <summary>
        /// Handle incoming register group messages. Creates a new group based upon the
        /// requested group Id and name.
        /// </summary>
        /// <param name="connection">The connection from which the request was sent.</param>
        /// <param name="message">The message containing the group Id and name.</param>
        private static void HandleRegisterGroup(Connection connection, Message message)
        {
            // Construct a basic response.
            Message response = new Message(MessageType.RegisterGroupResponse);

            Client client = Server.Instance.GetClient(message.ClientId);

            // Check if the client exists.
            if (client != null)
            {
                // Create new group, insert into database and add sender as initial member.
                Group newGroup = Server.Instance.CreateGroup(message.PayloadData);
                DatabaseHandler.AddClientToGroup(newGroup, client);
                newGroup.AddClient(client);

                // Return the Id and name of the new group.
                response.GroupID = newGroup.Id;
                response.PayloadData = newGroup.Name;
            }
            else
            {
                response.GroupID = -1;
            }
            
            connection.SendData(response);
        }

        /// <summary>
        /// Handle incoming group list requests. The server returns a list of groups
        /// encoded as a string with Id's and names together.
        /// </summary>
        /// <param name="connection">The connection from which the request was sent.</param>
        private static void HandleRequestGroups(Connection connection, Message message)
        {

            List<Group> groups = new List<Group>();
            foreach(Group group in Server.Instance.GetGroups())
            {
                if (!group.ContainsClient(message.ClientId))
                {
                    groups.Add(group);
                }
            }
            connection.SendData(new Message(MessageType.RequestGroupsResponse)
            {
                GroupList = groups.Cast<Shared.Group>().ToList()
            });
        }

        /// <summary>
        /// Handle incoming join group requests. Adds a client to a group based on the
        /// client Id and requested group Id.
        /// </summary>
        /// <param name="connection">The connection from which the request was sent.</param>
        /// <param name="message">The message containing the client Id and group Id.</param>
        private static void HandleJoinGroup(Connection connection, Message message)
        {
            // Get client by client Id.
            Client clientToAdd = Server.Instance.GetClient(message.ClientId);

            // Get the group and add the client to it.
            Group groupToJoin = Server.Instance.GetGroup(message.GroupID);

            if(clientToAdd != null && groupToJoin != null)
            {
                DatabaseHandler.AddClientToGroup(groupToJoin, clientToAdd);
                groupToJoin.AddClient(clientToAdd);
            }

            // TODO: Unsuccesful response.
            connection.SendData(new Message(MessageType.JoinGroupResponse)
            {
                GroupID = groupToJoin.Id,
                PayloadData = groupToJoin.Name
            });
        }

        /// <summary>
        /// Handle incoming leave group requests. Removes the client from a specified
        /// group.
        /// </summary>
        /// <param name="connection">The connection from which the request was sent.</param>
        /// <param name="message">The message containing the client Id and group Id.</param>
        private static void HandleLeaveGroup(Connection connection, Message message)
        {
            Client client = Server.Instance.GetClient(message.ClientId);

            Server.Instance.GetGroup(message.GroupID).RemoveClient(client);
        }

        /// <summary>
        /// Handle incoming chatmessages. Relay the chatmessage to all other clients in
        /// the group.
        /// </summary>
        /// <param name="connection">The connection from which the message was sent.</param>
        /// <param name="message">The message containing the chatmessage.</param>
        private static void HandleChatMessage(Connection connection, Message message)
        {
            //add message to database
            DatabaseHandler.AddMessage(message);
            // Relay the chatMessage to all other clients in the group.
            Server.Instance.GetGroup(message.GroupID).SendMessageToClients(message);
        }
    }
}
