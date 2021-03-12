using System;

namespace Messenger_Server
{
    public class JsonMessage
    {
        public int GroupID { get; set; }
        public string MessageType { get; set; }
        public JsonMessagePayload Payload { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class JsonMessagePayload
    {
        /// <summary>
        /// mime-type
        /// </summary>
        public string Type { get; set; }

        public string Data { get; set; }
    }
}
