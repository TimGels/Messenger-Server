using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger_Server
{
    public static class Helper
    {
        /// <summary>
        /// check if given password is right.
        /// TODO: Bcrypt for better validation
        /// </summary>
        /// <param name="client"></param>
        /// <param name="passwordToCheck"></param>
        /// <returns></returns>

        public static bool ValidatePassword(string email, string passwordToCheck)
        {
            string password = DatabaseHandler.GetPasswordFromClient(email);
            return passwordToCheck.Equals(password);
        }
    }
}
