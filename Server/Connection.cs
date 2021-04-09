using Shared;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger_Server
{
    /// <summary>
    /// Each client has one connection object associated with it.
    /// This class hold all the logic to send and read a message from the client.
    /// </summary>
    public class Connection : Shared.Connection
    {
        /// <summary>
        /// Create a new connection with the received TcpClient from TcpListener.Accept
        /// and a cancellationtoken to cancel the read operation.
        /// </summary>
        /// <param name="client">The received TcpClient.</param>
        /// <param name="cts">The cancellationtoken to cancel the read operation.</param>
        public Connection(TcpClient client, CancellationTokenSource cts)
            : base(client)
        {
            this.readerCts = cts;
        }

        /// <summary>
        /// Remove the client / connection association and close the TCP connection from
        /// the server side. This should result in an IOException when reading is performed.
        /// </summary>
        public override void Close()
        {
            lock (closedLock)
            {
                if (!closed)
                {
                    closed = true;

                    // "Logout" by removing the connection.
                    Server.Instance.DeleteConnection(this);

                    // Close the connection.
                    readerCts.Cancel();

                    client.Client.Shutdown(SocketShutdown.Both);
                    client.Client.Disconnect(false);
                    client.Close();
                    client.Dispose();
                    readerCts.Dispose();
                }
            }
        }

        public override async void ReadData()
        {
            try
            {
                while (true)
                {
                    // Buffer to hold the size of the message.
                    Byte[] sizeBuffer = new Byte[sizeof(Int32)];

                    // Read size of message into sizeBuffer. Blocks untill data is
                    // available. FIN will read 0 bytes, RST will throw.
                    if (await client.GetStream().ReadAsync(
                        sizeBuffer.AsMemory(0, sizeof(Int32)),
                        readerCts.Token)
                        > 0)
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

                        _ = Task.Run(() =>
                        {
                            string json = Encoding.ASCII.GetString(buffer);
                            CommunicationHandler.HandleMessage(this, new Message(json));
                        });
                    }
                    else
                    {
                        // Received FIN.
                        break;
                    }
                }
            }
            catch
            {
                // Received RST.
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

        /// <summary>
        /// Function for sending a message to the client.
        /// </summary>
        /// <param name="message"></param>
        public override void SendData(Message message)
        {
            try
            {
                if (!client.Connected || closed)
                {
                    return;
                }

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
