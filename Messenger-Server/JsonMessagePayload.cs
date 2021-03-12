using System;

namespace Messenger_Server
{
    public class JsonMessagePayload
    {
        /// <summary>
        /// mime-type
        /// </summary>
        public string Type { get; set; }

        public string Data { get; set; }
    }
}
