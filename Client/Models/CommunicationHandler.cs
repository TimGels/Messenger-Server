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
            }
        }
    }
}
