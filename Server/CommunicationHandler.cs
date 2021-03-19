using Shared;

namespace Messenger_Server
{
    public class CommunicationHandler
    {
        /// <summary>
        /// This method is called by the ReadData method in Client.
        /// It handles all incoming messages from clients.
        /// </summary>
        /// <param name="message">The incoming message.</param>
        /// <param name="client">The client where the message came from.</param>
        public static void HandleMessage(Connection connection, Message message)
        {
            switch (message.MessageType)
            {
                case MessageType.RegisterClient:
                    {
                        // create client from message
                        int retreivedid = 0;
                        Client client = new Client(retreivedid, null);
                        // add client in database / internal dictionary
                        Server.Instance.AddConnection(retreivedid, connection);
                    }
                    break;
                case MessageType.SignInClient:
                    {
                        // get client from message
                        Server.Instance.AddConnection(message.ClientId, connection);
                    }
                    break;
                case MessageType.ChatMessage:
                    // Relay the chatMessage to all other clients in the group.
                    Server.Instance.GetGroup(message.GroupID).SendMessageToClients(message);
                    break;
                case MessageType.RegisterGroup:
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
                    break;
            }
        }
    }
}
