using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Messenger_Server
{
    public class JsonMessage
    {
        public int GroupID { get; set; }
        public String MessageType { get; set; }
        public JsonMessagePayload Payload { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class JsonMessagePayload
    {
        /// <summary>
        /// mime-type
        /// </summary>
        public String Type { get; set; }

        public String Data { get; set; }
    }
}
