namespace Shared
{
    /// <summary>
    /// Convenience type to group login info together.
    /// </summary>
    public struct LoginStruct
    {
        /// <summary>
        /// Email address to use. Has to be unique.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password to use.
        /// </summary>
        public string Password { get; set; }
    }
}
