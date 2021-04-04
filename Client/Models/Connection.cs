using Shared;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger_Client.Models
{
    public class Connection : Shared.Connection
    {
        public Connection()
            : base(new TcpClient())
        {
            this.closed = true;
        }

        /// <summary>
        /// Attempts to opens the connection asynchronously. This should only take effect
        /// once.
        /// </summary>
        /// <returns>Task with result of the asynchronous connection attempt: True if
        /// opening connection succeeded or was already openend, false if connection could
        /// not be opened.</returns>
        public async Task<bool> OpenAsync()
        {
            // Checking here is faster and more elegant than throwing a SocketException
            // when already connected.
            if (client.Connected || !closed)
            {
                return true;
            }

            try
            {
                await client.ConnectAsync(Client.Instance.serverAddress, Client.Instance.port);
                closed = false;
                new Thread(ReadData).Start();
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("{0} failed with {1}",
                    MethodBase.GetCurrentMethod().Name,
                    e.GetType().FullName));
            }

            return client.Connected || !closed;
        }

        public override void ReadData()
        {
            try
            {
                while (true)
                {
                    // Buffer to hold the size of the message.
                    Byte[] sizeBuffer = new Byte[sizeof(Int32)];

                    // Read size of message into sizeBuffer. Blocks untill data is
                    // available. FIN will read 0 bytes, RST will throw.
                    if (client.GetStream().Read(sizeBuffer, 0, sizeof(Int32)) > 0)
                    {
                        // Convert the bytes into the size of the actual data, set
                        // ReceiveBufferSize and allocate enough spae to hold the data.
                        int total = BitConverter.ToInt32(sizeBuffer, 0);
                        client.ReceiveBufferSize = total;
                        Byte[] buffer = new byte[total];

                        // Loop through Receive() until enough data has been read and
                        // remember how much has been read.
                        int read = 0;
                        while (read < total)
                        {
                            // Determine amount to read based on amount of already read bytes.
                            int sizeToRead = total - read;

                            // If size is larger than available, only read what's available.
                            // Remaining data will be read by subsequent Receive() calls.
                            if (sizeToRead > client.Available)
                            {
                                sizeToRead = client.Available;
                            }

                            // Increment the read counter.
                            read += client.GetStream().Read(buffer, read, sizeToRead);
                        }

                        Task.Run(() =>
                        {
                            string json = Encoding.ASCII.GetString(buffer);
                            CommunicationHandler.HandleMessage(new Message(json));
                        });

                        Thread.Sleep(50);
                    }
                    else
                    {
                        Debug.WriteLine("Received FIN!");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("{0} failed with {1}",
                    MethodBase.GetCurrentMethod().Name,
                    e.GetType().FullName));
            }
        }
    }
}
