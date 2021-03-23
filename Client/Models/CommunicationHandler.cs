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
                case MessageType.RequestGroupsResponse:
                    List<Group> list = new List<Group>();
                    // Convert Messenger_Client.Group to Shared.Group.
                    message.GroupList.ForEach(group => list.Add(new Group(group)));

                    // TODO: assign to some viewmodel list (with writelock?).
                    break;
                case MessageType.RegisterClientResponse:

                    switch (message.ClientId) 
                    {
                        case -1:
                            Console.WriteLine("Naam is al bezet!");
                            break;
                        default:
                            Console.WriteLine("Account aangemaakt!");
                            Console.WriteLine(message.ClientId);
                            break;
                    }
                    break;
                case MessageType.SignInClientResponse:

                    switch (message.ClientId)
                    {
                        case -1:
                            Console.WriteLine("E-mail of wachtwoord verkeerd!");
                            break;
                        case -2:
                            Console.WriteLine("Already ingelogd!");
                            break;
                        default:
                            Console.WriteLine("Gefeliciteerd!");
                            break;
                    }
                    Console.WriteLine(message.ClientId);
                    break;
                case MessageType.SignOutClientResponse:

                    break;
                case MessageType.RegisterGroupResponse:
                    switch (message.GroupID) 
                    {
                        case -1:
                            Console.WriteLine("failed to create group");
                            break;
                        default:
                            Console.WriteLine("Group aangemaakt!");
                            break;
                    }
                    break;
                case MessageType.RequestGroupsResponse:

                    break;
            }
        }
    }
}
