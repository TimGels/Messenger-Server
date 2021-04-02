using System;
using System.Collections.Generic;
using Shared;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Messenger_Client.Models
{
    class CommunicationHandler
    {
        #region events

        public static event EventHandler<GroupListEventArgs> ObtainedRequestedGroups;
        public static event EventHandler JoinedGroup;
        public static event EventHandler<ResponseStateEventArgs> LogInResponse;
        public static event EventHandler<ResponseStateEventArgs> SignUpResponse;
        public static event EventHandler<ResponseStateEventArgs> RegisterGroupResponse;

        public class ResponseStateEventArgs : EventArgs
        {
            public int State { get; set; }
            public ResponseStateEventArgs(int state)
            {
                this.State = state;
            }
        }

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
            SignUpResponse?.Invoke(null, new ResponseStateEventArgs(message.ClientId));
        }

        private static void HandleSignInClientResponse(Message message)
        {
            if (message.ClientId >= 0)
            {
                Client.Instance.Id = message.ClientId;
                message.GroupList.ForEach(group => Client.Instance.AddGroup(new Group(group)));
            }
            LogInResponse?.Invoke(null, new ResponseStateEventArgs(message.ClientId));
        }

        private static void HandleRegisterGroupResponse(Message message)
        {
            if(message.GroupID > 0)
            {
                Client.Instance.AddGroup(new Group(message.GroupID, message.PayloadData));
            }
            RegisterGroupResponse?.Invoke(null, new ResponseStateEventArgs(message.ClientId));
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
                    //TODO: create pop up
                    break;
                default:
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
                _ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    group.AddMessage(message);
                });
            }
            else
            {
                Console.WriteLine("group doesn't exist");
                //TODO: doe iets wanneer de group niet bestaat
            }
        }
    }
}
