using Shared;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Messenger_Client.Models
{
    public class Connection : Shared.Connection
    {
        /// <summary>
        /// Invoked once the server disappears and the client is still connected.
        /// </summary>
        public event EventHandler ConnectionLost;

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
            try
            {
                if (client == null)
                {
                    client = new TcpClient();
                }

                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                string serverAddress = localSettings.Values["IPAddress"].ToString();
                int portNumber = Int32.Parse(localSettings.Values["PortNumber"].ToString());

                // Throws when already connected.
                await client.ConnectAsync(serverAddress, portNumber);

                // If control enters here, connecting succeeded and read thread is started.
                closed = false;
                readerCts = new CancellationTokenSource();
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

        /// <summary>
        /// Is the client connected to a remote host?
        /// </summary>
        /// <returns>True if connected, false if not.</returns>
        public bool IsConnected()
        {
            if (client is null)
            {
                return false;
            }

            return client.Connected || !closed;
        }

        /// <summary>
        /// Closes the connection without error. Will only take effect once.
        /// </summary>
        public override void Close()
        {
            CloseInternal(false);
        }

        /// <summary>
        /// Closes the connection, disposes the socket and the cancellationtoken. Will only
        /// take effect once.
        /// </summary>
        /// <param name="error">Indicates if an error occured before calling, such as a
        /// disposed socket or a failed Read(). If error has occured and CloseInternal
        /// has not been called yet, invokes the ConnectionLost event.</param>
        private void CloseInternal(bool error)
        {
            lock (closedLock)
            {
                if (!closed)
                {
                    closed = true;
                    readerCts.Cancel();
                    client.Client.Shutdown(SocketShutdown.Both);
                    client.Client.Disconnect(false);
                    client.Close();
                    client.Dispose();
                    client = null;
                    readerCts.Dispose();

                    if (error)
                    {
                        ConnectionLost?.Invoke(this, new EventArgs());
                    }
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
                        sizeBuffer,
                        0,
                        sizeof(Int32),
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

                CloseInternal(true);
            }
            finally
            {
                CloseInternal(false);
            }
        }

        /// <summary>
        /// Method for sending a message to the server.
        /// </summary>
        /// <param name="message"></param>
        public override void SendData(Message message)
        {
            try
            {
                // Can throw if client is already disposed.
                if (!client.Connected || closed)
                {
                    return;
                }

                //use base implementation.
                base.SendData(message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("{0} failed with {1}",
                    MethodBase.GetCurrentMethod().Name,
                    e.GetType().FullName));

                CloseInternal(true);
            }
        }
    }
}
