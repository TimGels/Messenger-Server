using Shared.Json;
using System;
using System.Collections.Generic;
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

        public string ImageString
        {
            get
            {
                if (PayloadType != null && PayloadType.Equals("image"))
                {
                    return jsonMessage.Payload.Data;
                }
                else
                {
                    return null;
                }
            }
        }

        public string PayloadData
        {
            get
            {
                if (ImageString == null)
                {
                    return jsonMessage.Payload.Data;
                }
                return null;
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

        public List<Group> GroupList
        {
            get
            {
                List<Group> groupList = new List<Group>();

                // Setting groups server-side with no registered groups works as expected:
                // empty string. But getter needs a check.
                if (!jsonMessage.Payload.Data.Equals(""))
                {
                    string[] groupStrings = jsonMessage.Payload.Data.Split(';');
                    foreach (string groupString in groupStrings)
                    {
                        groupList.Add(Group.FromFormatString(groupString));
                    }
                }

                return groupList;
            }
            set
            {
                List<string> list = new List<string>();
                foreach (Group group in value)
                {
                    list.Add(Group.ToFormatString(group));
                }
                jsonMessage.Payload.Data = String.Join(";", list.ToArray());
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
        /// returns a csv string representation of this message.
        /// </summary>
        /// <returns></returns>
        public string GetCsvString()
        {
            return this.GroupID + "," + this.PayloadType + "," + this.PayloadData + "," + this.ClientId + "," + this.ClientName + "," + DateTime.ToString() + "\n";
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
        ChatMessage,
        KeepAlive
    }
}
