using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public class CommunicationHandler
    {
        private static JsonMessage Parse(String jsonString)
        {
            JsonSerializerOptions deserializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<JsonMessage>(jsonString, deserializerOptions);
        }

        /// <summary>
        /// This method is called by the ReadData method in Client.
        /// It handles all incoming messages from clients.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="jsonString"></param>
        public static void HandleMessage(Message message)
        {
            
            if (message.MessageType.Equals("chatMessage"))
            {
                
                Server.Instance.GetGroup(message.GroupID).SendMessageToClients(message);
            }
            else if (message.MessageType.Equals("registerGroup"))
            {
                int groupId = Server.Instance.CreateGroup(message.PayloadData);
                Message m = new Message()
                {
                    GroupID = groupId,
                    MessageType = "registerGroupResponse"
                };
                message.Sender.SendData(m);
            }
        }

        public static string SerializeMessage(Message message)
        {
            return JsonSerializer.Serialize(message.jsonMessage);
        }
    }
}
