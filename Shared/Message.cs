using System;
using System.Drawing;
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

        public MessageType MessageType
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
                return Helper.Base64StringToImage(jsonMessage.Payload.Data);
            }
            set
            {
                //TODO: also set the Payload.Type property?
                this.jsonMessage.Payload.Data = Helper.ImageTobase64String(value);
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
                return Helper.Base64ToString(jsonMessage.Client.Name);
            }
            set
            {
                jsonMessage.Client.Name = Helper.StringToBase64(value);
            }
        }

        public LoginStruct LoginInfo
        {
            get
            {
                string[] splitted = jsonMessage.Payload.Data.Split(';');
                return new LoginStruct()
                {
                    Email = Helper.Base64ToString(splitted[0]),
                    Password = Helper.Base64ToString(splitted[1])
                };
            }
            set
            {
                jsonMessage.Payload.Data = String.Format("{0};{1}",
                    Helper.StringToBase64(value.Email),
                    Helper.StringToBase64(value.Password));
            }
        }

        public RegisterStruct RegisterInfo
        {
            get
            {
                return new RegisterStruct()
                {
                    Login = LoginInfo,
                    Username = ClientName
                };
            }
            set
            {
                LoginInfo = value.Login;
                ClientName = value.Username;
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

    /// <summary>
    /// The type of message indicates the intent of the sender.
    /// </summary>
    public enum MessageType
    {
        RegisterClient,
        RegisterClientResponse,
        SignInClient,
        SignInClientResponse,
        SignOutClient,
        SignOutClientResponse,
        RegisterGroup,
        RegisterGroupResponse,
        RequestGroups,
        RequestGroupsResponse,
        JoinGroup,
        JoinGroupResponse,
        LeaveGroup,
        ChatMessage
    }
}
