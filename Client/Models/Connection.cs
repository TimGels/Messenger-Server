using Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger_Client.Models
{
    public class Connection : Shared.Connection
    {
        public Connection(string serverAddress, int port)
            : base(new TcpClient(serverAddress, port))
        {
            //start readerThread
            new Thread(ReadData).Start();
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
