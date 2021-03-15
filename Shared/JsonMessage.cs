using System;

namespace Shared
{
    internal class JsonMessage
    {
        public int GroupID { get; set; }
        public string MessageType { get; set; }
        public JsonMessagePayload Payload { get; set; }
        public JsonMessageClient Client { get; set; }
        public DateTime DateTime { get; set; }

        public JsonMessage()
        {
            this.Payload = new JsonMessagePayload();
            this.Client = new JsonMessageClient();
        }
    }

}
