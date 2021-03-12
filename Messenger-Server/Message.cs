using System.Text.Json;

namespace Messenger_Server
{
    public class Message
    {
        //TODO dateTime.
        public Client Sender { get; set; }
        public string MessageType
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

        private readonly JsonMessage jsonMessage;

        public Message(Client s, string json)
        {
            this.Sender = s;
            this.jsonMessage = Parse(json);
        }

        public Message()
        {
            this.jsonMessage = new JsonMessage();
        }

        /// <summary>
        /// Convert a string with JSON data to a JsonMessage object.
        /// </summary>
        /// <param name="jsonString">The string to convert.</param>
        /// <returns>The JsonMessage object.</returns>
        private static JsonMessage Parse(string jsonString)
        {
            JsonSerializerOptions deserializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<JsonMessage>(jsonString, deserializerOptions);
        }

        /// <summary>
        /// Serialize a message to a JSON string representation.
        /// </summary>
        /// <param name="m"></param>
        /// <returns>The JSON string.</returns>
        public static string SerializeMessage(Message m)
        {
            return JsonSerializer.Serialize(m.jsonMessage);
        }

        //TODO: on the client, a new message has to be constructed. The idea is, that this message class, is the abstractionlayer of JsonMessage.
        // in this way, al other client doesnt have to know about the usage of jsonMessage.
        public Message(Group g)
        {

        }
    }
}
