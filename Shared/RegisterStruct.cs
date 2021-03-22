namespace Shared
{
    /// <summary>
    /// Convenience type to group registration info together.
    /// </summary>
    public struct RegisterStruct
    {
        /// <summary>
        /// The username to register.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Contains info for logging in upon succesful registration.
        /// </summary>
        public LoginStruct Login { get; set; }
    }
}
