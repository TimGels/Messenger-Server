using System;
using System.Collections.Generic;
using System.Diagnostics;
using Shared;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Messenger_Client.Models
{
    class CommunicationHandler
    {

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
                case MessageType.KeepAlive:
                    HandleKeepAliveMessage(message);
                    break;
            }
        }

        public static event EventHandler<ResponseStateEventArgs> LoggedInResponse;
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

        internal static void SendRegisterMessage(string mail, string name, string password)
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
            LoggedInResponse?.Invoke(null, new ResponseStateEventArgs(message.ClientId));
        }

        private static void HandleRegisterGroupResponse(Message message)
        {
            RegisterGroupResponse?.Invoke(null, new ResponseStateEventArgs(message.ClientId));
        }
        
        private static void HandleRequestGroupsResponse(Message message)
        {
            List<Group> list = new List<Group>();
            // Convert Messenger_Client.Group to Shared.Group.
            message.GroupList.ForEach(group => list.Add(new Group(group)));

            // TODO: assign to some viewmodel list (with writelock?).
        }

        private static void HandleJoinGroupResponse(Message message)
        {
            switch (message.GroupID)
            {
                case -1:
                    Console.WriteLine("something went wrong!");
                    //TODO: create pop up
                    break;
                default:
                    // Run on UI thread to prevent COMException.
                    _ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        Client.Instance.AddGroup(new Group(message.GroupID, message.PayloadData));
                    });
                    Console.WriteLine("joined a group");
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

        /// <summary>
        /// Bounce the keepalive message back to the server.
        /// </summary>
        /// <param name="message"></param>
        private static void HandleKeepAliveMessage(Message message)
        {
            Client.Instance.Connection.SendData(message);
        }
    }
}
