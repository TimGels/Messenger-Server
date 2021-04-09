using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Messenger_Client.Services
{
    public static class ImageUtilityService
    {
        /// <summary>
        /// Converts a <see cref="IRandomAccessStream"> stream</see> to a <see cref="byte">byte</see> array.
        /// </summary>
        /// <param name="s"></param>
        /// <returns><see cref="Task{byte[]}"/></returns>
        public static async Task<byte[]> ConvertStreamToByteArray(IRandomAccessStream s)
        {
            DataReader dataReader = new DataReader(s.GetInputStreamAt(0));
            byte[] bytes = new byte[s.Size];
            await dataReader.LoadAsync((uint)s.Size);
            dataReader.ReadBytes(bytes);
            return bytes;
        }
    }
}
