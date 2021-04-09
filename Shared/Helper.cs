using System;
using System.Text;

namespace Shared
{
    internal static class Helper
    {
        /// <summary>
        /// Encoder for a base64 to a decoded string
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        internal static string Base64ToString(string encoded)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        }

        /// <summary>
        /// Method for encoding a string to a base64 string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string StringToBase64(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }
    }
}
