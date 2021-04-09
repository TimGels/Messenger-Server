namespace Shared.Json
{
    /// <summary>
    /// Representation of the client JSON object. Contains info about the original sender
    /// of the message.
    /// </summary>
    internal class JsonMessageClient
    {
        /// <summary>
        /// <see cref="Message.ClientId"/>
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// <see cref="Message.ClientName"/>
        /// </summary>
        public string Name { get; set; }
    }
}
