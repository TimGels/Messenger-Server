using System;
using System.Collections.Generic;
using Shared;

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
        

        private static void HandleRegisterClientResponse(Message message)
        {
            switch (message.ClientId)
            {
                case -1:
                    Console.WriteLine("Naam is al bezet!");
                    break;
                default:
                    Console.WriteLine("Account aangemaakt!");
                    Client.Instance.Id = message.ClientId;
                    break;
            }
        }

        private static void HandleSignInClientResponse(Message message)
        {
            switch (message.ClientId)
            {
                case -1:
                    Console.WriteLine("E-mail of wachtwoord verkeerd!");
                    break;
                case -2:
                    Console.WriteLine("Already ingelogd!");
                    break;
                default:
                    message.GroupList.ForEach(group => Client.Instance.AddGroup(new Group(group)));
                    Console.WriteLine("Gefeliciteerd!");
                    break;
            }
            Console.WriteLine(message.ClientId);
        }

        private static void HandleRegisterGroupResponse(Message message)
        {
            switch (message.GroupID)
            {
                case -1:
                    Console.WriteLine("failed to create group");
                    break;
                default:
                    Client.Instance.AddGroup(new Group(message.GroupID, message.PayloadData));
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
                    Console.WriteLine("joined a group");
                    Client.Instance.AddGroup(new Group(message.GroupID, message.PayloadData));
                    break;
            }
        }

        private static void HandleChatMessage(Message message)
        {
            List<Group> groups = Client.Instance.Groups;
            foreach(Group group in groups)
            {
                if(group.Id == message.GroupID)
                {
                    group.AddMessage(message);
                }
            }
        }
    }
}
