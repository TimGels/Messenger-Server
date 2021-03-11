namespace Messenger_Server
{
    public class CommunicationHandler
    {
        /// <summary>
        /// This method is called by the ReadData method in Client.
        /// It handles all incoming messages from clients.
        /// </summary>
        /// <param name="message">The incoming message.</param>
        public static void HandleMessage(Message message)
        {
            if (message.MessageType.Equals("chatMessage"))
            {
                // Relay the chatMessage to all other clients in the group.
                Server.Instance.GetGroup(message.GroupID).SendMessageToClients(message);
            }
            else if (message.MessageType.Equals("registerGroup"))
            {
                // Create a new group, add the sender as initial group member
                // and return the ID of the new group.
                Group newGroup = Server.Instance.CreateGroup(message.PayloadData);
                newGroup.AddClient(message.Sender);
                Message m = new Message()
                {
                    GroupID = newGroup.GroupID,
                    MessageType = "registerGroupResponse"
                };
                message.Sender.SendData(m);
            }
        }
    }
}
