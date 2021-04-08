namespace Messenger_Server
{
    public static class Helper
    {
        /// <summary>
        /// Check if the password for the given email is correct.
        /// </summary>
        /// <param name="email">The email for which to get the correct password.</param>
        /// <param name="passwordToCheck">The password to verify.</param>
        /// <returns>True if the password which belongs to the given email corresponds
        /// to the given password. False if the given password is incorrect or the given
        /// email doesn't exist in the database.</returns>
        public static bool ValidatePassword(string email, string passwordToCheck)
        {
            string databasePassword = DatabaseHandler.GetPasswordFromClient(email);
            if (databasePassword != null)
            {
                return BCrypt.Net.BCrypt.Verify(passwordToCheck, databasePassword);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Method for hasing a given password
        /// </summary>
        /// <param name="passwordToHash">Hashed password</param>
        /// <returns></returns>
        public static string HashPassword(string passwordToHash)
        {
            return BCrypt.Net.BCrypt.HashPassword(passwordToHash);
        }
    }
}
