using System;
using System.Text.Json;

namespace Shared
{
    /// <summary>
    /// Abstraction layer on top of the JSON message representation. Responsible for
    /// converting from and to JSON.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Internal JSON message representation of this message.
        /// </summary>
        internal readonly JsonMessage jsonMessage;

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
            get
            {
                return jsonMessage.GroupID;
            }
            set
            {
                jsonMessage.GroupID = value;
            }
        }

        public string PayloadData
        {
            get
            {
                return jsonMessage.Payload.Data;
            }
            set
            {
                jsonMessage.Payload.Data = value;
            }
        }

        public string PayloadType
        {
            get
            {
                return jsonMessage.Payload.Type;
            }
            set
            {
                jsonMessage.Payload.Type = value;
            }
        }
        public int ClientId
        {
            get
            {
                return this.jsonMessage.Client.Id;
            }
            set
            {
                this.jsonMessage.Client.Id = value;
            }
        }

        public string ClientName
        {
            get
            {
                return this.jsonMessage.Client.Name;
            }
            set
            {
                this.jsonMessage.Client.Name = value;
            }
        }

        public DateTime DateTime
        {
            get
            {
                return this.jsonMessage.DateTime;
            }
            set
            {
                this.jsonMessage.DateTime = value;
            }
        }

        /// <summary>
        /// Construct a message based on a JSON string representation.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        public Message(string json)
        {
            this.jsonMessage = Parse(json);
        }

        /// <summary>
        /// Construct a blank message.
        /// </summary>
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
        /// <param name="message">The message to serialize.</param>
        /// <returns>The JSON string.</returns>
        public static string SerializeMessage(Message message)
        {
            return JsonSerializer.Serialize(message.jsonMessage);
        }
    }
}
