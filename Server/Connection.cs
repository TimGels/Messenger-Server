using Shared;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Messenger_Server
{
    public class Connection : Shared.Connection
    {
        /// <summary>
        /// Indicates if the current connection has been closed already.
        /// </summary>
        private bool closed = false;

        /// <summary>
        /// Since Close() can be called from the Read() thread or the CommuncationHandler
        /// thread, this lock is used to make sure the Close() only executes once.
        /// </summary>
        private readonly object closedLock = new object();

        public Connection(TcpClient client)
            : base(client)
        {

        }

        /// <summary>
        /// Remove the client / connection association and close the TCP connection from
        /// the server side. This should result in an IOException when reading is performed.
        /// </summary>
        public void Close()
        {
            lock (closedLock)
            {
                if (!closed)
                {
                    closed = true;

                    // "Logout" by removing the connection.
                    Server.Instance.DeleteConnection(this);

                    // Close the connection.
                    client.Close();
                    client.Dispose();
                }
            }
        }

        public override void ReadData()
        {
            try
            {
                // Don't lock closed since it is not crucial to keep going once more.
                while (client.Connected && !closed)
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
                            CommunicationHandler.HandleMessage(this, new Message(json));
                        });
                    }
                    else
                    {
                        Console.WriteLine("Received FIN!");
                        break;
                    }

                    Thread.Sleep(50);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("{0} failed with {1}",
                    MethodBase.GetCurrentMethod().Name,
                    e.GetType().FullName));
            }
            finally
            {
                // Client abruptly disconnected or sent signout message. In case of the
                // former, we need to close the connection ourselves.
                // In case of the latter, the closing has already been done and is the
                // reason we're here, so Close() should not have any effect.
                Close();
            }
        }

        public override void SendData(Message message)
        {

            // Check connection state.
            if (!client.Connected || closed)
            {
                return;
            }

            try
            {
                base.SendData(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("{0} failed with {1}",
                    MethodBase.GetCurrentMethod().Name,
                    e.GetType().FullName));

                // Close the connection and shutdown on exception.
                Close();
            }
        }
    }
}
