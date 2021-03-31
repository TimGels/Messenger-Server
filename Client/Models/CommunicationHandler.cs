using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security;
using System.Threading;
using Shared;

namespace Messenger_Client.Models
{
    class CommunicationHandler
    {
        #region events
        public static event EventHandler LoggedInSuccesfully;
        public static event EventHandler<GroupListEventArgs> ObtainedRequestedGroups;
        public static event EventHandler JoinedGroup;

        public class GroupListEventArgs : EventArgs
        {
            public List<Group> Groups { get; set; }
            public GroupListEventArgs(List<Group> groups)
            {
                Groups = groups;
            }
        }
        #endregion

        public static void HandleMessage(Message message)
        {
            switch (message.MessageType)
            {
                case MessageType.RegisterClientResponse:
                    HandleRegisterClientResponse(message);
                    break;
                case MessageType.SignInClientResponse:
                    HandleSignInClientResponse(message);
                    break;
                case MessageType.RegisterGroupResponse:
                    HandleRegisterGroupResponse(message);
                    break;
                case MessageType.RequestGroupsResponse:
                    HandleRequestGroupsResponse(message);
                    break;
                case MessageType.JoinGroupResponse:
                    HandleJoinGroupResponse(message);
                    break;
                case MessageType.ChatMessage:
                    HandleChatMessage(message);
                    break;
            }
        }
        #region send message methods


        public static void SendJoinGroupMessage(int groupId)
        {
            Client.Instance.Connection.SendData(new Message()
            {
                MessageType = MessageType.JoinGroup,
                GroupID = groupId,
                ClientId = Client.Instance.Id
            });
        }

        public static void SendRequestGroupMessages()
        {
            Client.Instance.Connection.SendData(new Message()
            {
                MessageType = MessageType.RequestGroups,
                ClientId = Client.Instance.Id
            });
        }


        public static void SendLoginMessage(string email, string password)
        {
            Client.Instance.Connection.SendData(new Message()
            {
                MessageType = MessageType.SignInClient,
                LoginInfo = new LoginStruct()
                {
                    Email = email,
                    Password = password
                }
            });
        }

        public static void SendRegisterMessage(string mail, string name, string password)
        {
            Client.Instance.Connection.SendData(new Message()
            {
                MessageType = MessageType.RegisterClient,
                RegisterInfo = new RegisterStruct()
                {
                    Username = name,
                    Login = new LoginStruct()
                    {
                        Email = mail,
                        Password = password
                    }
                }
            });
        }

        public static void SendRegisterGroupMessage(string name) {
            Client.Instance.Connection.SendData(new Message()
            {
                ClientId = Client.Instance.Id,
                MessageType = MessageType.RegisterGroup,
                PayloadData = name
            });
        }
        #endregion

        #region handle incoming messages


        private static void HandleRegisterClientResponse(Message message)
        {
            switch (message.ClientId)
            {
                case -1:
                    Debug.WriteLine("e-mail al in gebruik");
                    //TODO: geef melding dat e-mail al in gebruik is.
                    break;
                default:
                    //TODO mooie pop up ofso
                    Debug.WriteLine("Account aangemaakt!");
                    Debug.WriteLine("ClientID: " + message.ClientId);

                    break;
            }
        }

        private static void HandleSignInClientResponse(Message message)
        {
            switch (message.ClientId)
            {
                case -1:
                    Debug.WriteLine("E-mail of wachtwoord verkeerd!");
                    //TODO: geef melding dat de combinatie van e-mail en wachtwoord verkeerd is
                    break;
                case -2:
                    Debug.WriteLine("Already ingelogd!");
                    //TODO: geef melding dat het account al ergens anders is ingelogd
                    break;
                default:
                    Debug.WriteLine("Gefeliciteerd!");
                    Client.Instance.Id = message.ClientId;
                    message.GroupList.ForEach(group => Client.Instance.AddGroup(new Group(group)));
                    LoggedInSuccesfully?.Invoke(null, null);
                    break;
            }
        }

        private static void HandleRegisterGroupResponse(Message message)
        {
            switch (message.GroupID)
            {
                case -1:
                    Debug.WriteLine("failed to create group");
                    //TODO: geef melding dat het aanmaken van een group mislukt is
                    break;
                default:
                    Client.Instance.AddGroup(new Group(message.GroupID, message.PayloadData));
                    Console.WriteLine("Group aangemaakt!");
                    break;
            }
        }
        
        private static void HandleRequestGroupsResponse(Message message)
        {
            List<Group> groups = new List<Group>();
            // Convert Messenger_Client.Group to Shared.Group.
            message.GroupList.ForEach(group => groups.Add(new Group(group)));

            ObtainedRequestedGroups?.Invoke(null, new GroupListEventArgs(groups));
        }

        private static void HandleJoinGroupResponse(Message message)
        {
            switch (message.GroupID)
            {
                case -1:
                    Debug.WriteLine("something went wrong!");
                    //TODO: create pop up
                    break;
                default:
                    Debug.WriteLine("joined a group");
                    Client.Instance.AddGroup(new Group(message.GroupID, message.PayloadData));
                    JoinedGroup?.Invoke(null, null);
                    break;
            }
        }

        private static void HandleChatMessage(Message message)
        {
            Group group = Client.Instance.GetGroup(message.GroupID);
            if (group != null)
            {
                group.AddMessage(message);
            }
            else
            {
                Console.WriteLine("group doesn't exist");
                //TODO: doe iets wanneer de group niet bestaat
            }
        }

        #endregion
    }
}
