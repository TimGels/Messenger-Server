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

        public static void HandleMessage(Client client, String jsonString)
        {
            JsonMessage jsonMessageObject = Parse(jsonString);
            if (jsonMessageObject.MessageType.Equals("groupMessage"))
            {
                //Server.Instance.
            }
        }
    }
}
