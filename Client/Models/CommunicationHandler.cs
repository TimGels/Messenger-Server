using Shared;
using System;
using System.Collections.Generic;

namespace Messenger_Client.Models
{
    public static class CommunicationHandler
    {
        #region events

        /// <summary>
        /// This event is used when there were group returned from the requestGroups message.
        /// </summary>
        public static event EventHandler<GroupListEventArgs> ObtainedRequestedGroups;

        /// <summary>
        /// This event is raised when the client succesfully joined a group.
        /// </summary>
        public static event EventHandler JoinedGroup;

        /// <summary>
        /// This event is raised when there was a response for the logging in action.
        /// </summary>
        public static event EventHandler<ResponseStateEventArgs> LogInResponse;

        /// <summary>
        /// This event is raised when there came a reponse on the sign up action.
        /// </summary>
        public static event EventHandler<ResponseStateEventArgs> SignUpResponse;

        /// <summary>
        /// This event is raised when there was a response received for the registering group action.
        /// </summary>
        public static event EventHandler<ResponseStateEventArgs> RegisterGroupResponse;

        /// <summary>
        /// This EventArgs is used for sending the state/response of an action to the handling method.
        /// In this way, the handling method can do several action based on the response.
        /// </summary>
        public class ResponseStateEventArgs : EventArgs
        {
            public int State { get; set; }
            public ResponseStateEventArgs(int state)
            {
                this.State = state;
            }
        }

        /// <summary>
        /// This EventArgs is used to send obtained groups to the handling method.
        /// </summary>
        public class GroupListEventArgs : EventArgs
        {
            public List<Group> Groups { get; set; }
            public GroupListEventArgs(List<Group> groups)
            {
                Groups = groups;
            }
        }
        #endregion

        /// <summary>
        /// This method handles incoming messages based on a message's messageType.
        /// </summary>
        /// <param name="message"></param>
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

        /// <summary>
        /// Method for sending a leave group message to the server.
        /// </summary>
        /// <param name="groupId">The id of the group to leave.</param>
        public static void SendLeaveGroupMessage(int groupId)
        {
            Client.Instance.Connection.SendData(new Message(MessageType.LeaveGroup)
            {
                GroupID = groupId,
                ClientId = Client.Instance.Id
            });
        }

        /// <summary>
        /// Method for sending a join group message to the server.
        /// </summary>
        /// <param name="groupId">The id of the group to join.</param>
        public static void SendJoinGroupMessage(int groupId)
        {
            Client.Instance.Connection.SendData(new Message(MessageType.JoinGroup)
            {
                GroupID = groupId,
                ClientId = Client.Instance.Id
            });
        }

        /// <summary>
        /// Method for sending a request groups message to the server.
        /// </summary>
        public static void SendRequestGroupsMessages()
        {
            Client.Instance.Connection.SendData(new Message(MessageType.RequestGroups)
            {
                ClientId = Client.Instance.Id
            });
        }

        /// <summary>
        /// Send Login message to server.
        /// </summary>
        /// <param name="email">Email filled in by the user.</param>
        /// <param name="password">Password filled in by the user.</param>
        public static void SendLoginMessage(string email, string password)
        {
            Client.Instance.Connection.SendData(new Message(MessageType.SignInClient)
            {
                LoginInfo = new LoginStruct()
                {
                    Email = email,
                    Password = password
                }
            });
        }

        /// <summary>
        /// Sending a register message to the server.
        /// </summary>
        /// <param name="mail">The mail to register the client with.</param>
        /// <param name="name">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        public static void SendRegisterMessage(string mail, string name, string password)
        {
            Client.Instance.Connection.SendData(new Message(MessageType.RegisterClient)
            {
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

        /// <summary>
        /// Method for sending a register group message to the server.
        /// </summary>
        /// <param name="name">The name of the group to register.</param>
        public static void SendRegisterGroupMessage(string name)
        {
            Client.Instance.Connection.SendData(new Message(MessageType.RegisterGroup)
            {
                ClientId = Client.Instance.Id,
                PayloadData = name
            });
        }

        #endregion

        #region handle incoming messages

        /// <summary>
        /// Handle a Register Client Response message.
        /// This method will invoke the SignUpResponse event.
        /// </summary>
        /// <param name="message"></param>
        private static void HandleRegisterClientResponse(Message message)
        {
            SignUpResponse?.Invoke(null, new ResponseStateEventArgs(message.ClientId));
        }

        /// <summary>
        /// Handle the sign in client response message. If the login was succesfully, the id and name are set.
        /// </summary>
        /// <param name="message"></param>
        private static void HandleSignInClientResponse(Message message)
        {
            //if client message is > 0, the login action was succesfully.
            if (message.ClientId > 0)
            {
                Client.Instance.Id = message.ClientId;
                Client.Instance.Name = message.ClientName;
                message.GroupList.ForEach(group => Client.Instance.AddGroup(new Group(group)));
            }
            LogInResponse?.Invoke(null, new ResponseStateEventArgs(message.ClientId));
        }

        /// <summary>
        /// Handle the register group response
        /// </summary>
        /// <param name="message"></param>
        private static void HandleRegisterGroupResponse(Message message)
        {
            //if obtained groupID > 0 then the registering was succefully: create new group and add it to the groups.
            if (message.GroupID > 0)
            {
                Client.Instance.AddGroup(new Group(message.GroupID, message.PayloadData));
            }
            RegisterGroupResponse?.Invoke(null, new ResponseStateEventArgs(message.ClientId));
        }

        /// <summary>
        /// Handle the request group response.
        /// </summary>
        /// <param name="message"></param>
        private static void HandleRequestGroupsResponse(Message message)
        {
            List<Group> groups = new List<Group>();
            message.GroupList.ForEach(group => groups.Add(new Group(group)));

            ObtainedRequestedGroups?.Invoke(null, new GroupListEventArgs(groups));
        }

        /// <summary>
        /// Handle the join group response.
        /// </summary>
        /// <param name="message"></param>
        private static void HandleJoinGroupResponse(Message message)
        {
            switch (message.GroupID)
            {
                case -1:
                    // this is following the protocol. 
                    // But in practice the join group action cannot go wrong.
                    break;
                default:
                    Client.Instance.AddGroup(new Group(message.GroupID, message.PayloadData));
                    JoinedGroup?.Invoke(null, null);
                    break;
            }
        }

        /// <summary>
        /// Handle an incoming chat message.
        /// </summary>
        /// <param name="message"></param>
        private static void HandleChatMessage(Message message)
        {
            // Get the group the message is for.
            Group group = Client.Instance.GetGroup(message.GroupID);
            if (group != null)
            {
                // Add the message to the group.
                group.AddMessage(message);
            }
        }
        #endregion

    }
}
