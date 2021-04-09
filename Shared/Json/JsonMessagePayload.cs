namespace Shared.Json
{
    /// <summary>
    /// Representation of the payload JSON object. Contains the raw data including some 
    /// meta-info.
    /// </summary>
    internal class JsonMessagePayload
    {
        /// <summary>
        /// <see cref="Message.PayloadType"/>
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// <see cref="Message.PayloadData"/>
        /// </summary>
        public string Data { get; set; }
    }
}
