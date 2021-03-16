using System;
using System.Drawing;
using System.IO;
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

        public Image Image
        {
            get
            {
                //TODO: check if data is from the type image?
                return Base64StringToImage(this.jsonMessage.Payload.Data);
            }
            set
            {
                //TODO: also set the Payload.Type property?
                this.jsonMessage.Payload.Data = ImageTobase64String(value);
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

        /// <summary>
        /// Function for converting an image to a base64 string.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static string ImageTobase64String(Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                byte[] imageBytes = stream.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);

                return base64String;
            }
        }

        /// <summary>
        /// Function for converting a base64 string to an image object.
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        private static Image Base64StringToImage(string base64)
        {
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64)))
            {
                Image i = System.Drawing.Image.FromStream(stream);
                return i;
            }
        }
    }
}
