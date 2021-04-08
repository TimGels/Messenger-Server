using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Messenger_Client.Services
{
    public static class ImageUtilityService
    {
        /// <summary>
        /// Converts a <see cref="IRandomAccessStream"> stream</see> to a <see cref="byte">byte</see> array.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static async Task<byte[]> ConvertStreamToByteArray(IRandomAccessStream s)
        {
            var dr = new DataReader(s.GetInputStreamAt(0));
            var bytes = new byte[s.Size];
            await dr.LoadAsync((uint)s.Size);
            dr.ReadBytes(bytes);
            return bytes;
        }
    }
}
