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
            }
        }

        public static event EventHandler LoggedInSuccesfully;

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
                    // AddGroup throwns no COMException since no ViewModel with
                    // ObservableList<Group> has been constructed at this point.
                    // Could be problematic on Logout->Login.
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
                    // Run on UI thread to prevent COMException.
                    _ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Idle, () =>
                    {
                        Client.Instance.AddGroup(new Group(message.GroupID, message.PayloadData));
                    });
                    Console.WriteLine("Group aangemaakt!");
                    break;
            }
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
                    _ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Idle, () =>
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
                group.AddMessage(message);
            }
            else
            {
                Console.WriteLine("group doesn't exist");
                //TODO: doe iets wanneer de group niet bestaat
            }
        }
    }
}
