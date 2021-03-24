using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Shared
{
    internal static class Helper
    {
        /// <summary>
        /// Function for converting an image to a base64 string.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        internal static string ImageTobase64String(Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                byte[] imageBytes = stream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        /// <summary>
        /// Function for converting a base64 string to an image object.
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        internal static Image Base64StringToImage(string base64)
        {
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64)))
            {
                return Image.FromStream(stream);
            }
        }

        internal static string Base64ToString(string encoded)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        }

        internal static string StringToBase64(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }
    }
}
