using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public class Message
    {
        //TODO dateTime.
        public Client Sender { get; set; }
        public String MessageType
        {
            get
            {
                return jsonMessage.MessageType;
            }
            set
            {
                jsonMessage.MessageType = value;
            }
        }
        public int GroupID
        {
            get {
                return jsonMessage.GroupID;
            }
            set {
                jsonMessage.GroupID = value;
            }
        }

        public string PayloadData
        {
            get {
                return jsonMessage.Payload.Data;
            }
            set {
                jsonMessage.Payload.Data = value;
            }
        }

        public string PayloadType
        {
            get {
                return jsonMessage.Payload.Type;
            }
            set {
                jsonMessage.Payload.Type = value;
            }
        }

        private JsonMessage jsonMessage;

        public Message(Client s, String json)
        {
            this.Sender = s;
            this.jsonMessage = Parse(json);
        }

        public Message()
        {
            this.jsonMessage = new JsonMessage();
        }

        private static JsonMessage Parse(String jsonString)
        {
            JsonSerializerOptions deserializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<JsonMessage>(jsonString, deserializerOptions);
        }
        public static string SerializeMessage(Message m)
        {
            return JsonSerializer.Serialize(m.jsonMessage);
        }

        //TODO: on the client, a new message has to be constructed. The idea is, that this message class, is the abstractionlayer of JsonMessage.
        // in this way, al other client doesnt have to know about the usage of jsonMessage.
        public Message(Group g)
        {

        }

        public void addPayload(String data)
        {
            this.jsonMessage.Payload.Data = data;
        }
    }
}
