using System;

namespace Shared.Json
{
    /// <summary>
    /// Top-level representation of the JSON structure used to send messages.
    /// </summary>
    internal class JsonMessage
    {
        /// <summary>
        /// <see cref="Message.GroupID"/>
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// <see cref="Shared.MessageType"/>
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// <see cref="Message.PayloadType"/>
        /// </summary>
        public JsonMessagePayload Payload { get; set; }

        /// <summary>
        /// <see cref="JsonMessageClient"/>
        /// </summary>
        public JsonMessageClient Client { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Base constructor.
        /// </summary>
        public JsonMessage()
        {
            this.Payload = new JsonMessagePayload();
            this.Client = new JsonMessageClient();
        }
    }

}
