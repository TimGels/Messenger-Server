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

        /// <summary>
        /// Timer to check for an active connection.
        /// </summary>
        private readonly Timer keepAliveTimer = new Timer(5 * 1000);

        /// <summary>
        /// Indicates whether a keepalive has been received.
        /// </summary>
        private bool keepAliveReceived = true;

        public Connection(TcpClient client)
            : base(client)
        {
            CommunicationHandler.KeepAliveReceived += OnKeepAliveReceived;

            keepAliveTimer.Elapsed += OnKeepaliveTimerElapsed;
            keepAliveTimer.Start();
        }

        /// <summary>
        /// Indicates the keepalive from the client has been received.
        /// </summary>
        /// <param name="sender">Should be null.</param>
        /// <param name="e">Should be null.</param>
        private void OnKeepAliveReceived(object sender, EventArgs e)
        {
            keepAliveReceived = true;
        }

        /// <summary>
        /// Invoked when timer elapsed. Checks if keepalive has been received. If so,
        /// resend keepalive. If not, Close() the connection.
        /// </summary>
        /// <param name="sender">Should be null.</param>
        /// <param name="e">Should be null.</param>
        private void OnKeepaliveTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (closedLock)
            {
                if (closed)
                {
                    return;
                }
            }

            // Check if client has responded.
            if (!keepAliveReceived)
            {
                Close();
            }
            else
            {
                keepAliveReceived = false;

                SendData(new Message()
                {
                    MessageType = MessageType.KeepAlive
                });
            }
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

                    // Remove event handlers and stop timer.
                    CommunicationHandler.KeepAliveReceived -= OnKeepAliveReceived;
                    keepAliveTimer.Elapsed -= OnKeepaliveTimerElapsed;
                    keepAliveTimer.Stop();
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
                    if (client.GetStream().DataAvailable)
                    {
                        // Result from multiple Read() calls will be stored here.
                        string result = null;

                        // Call Read() untill newline received.
                        while (true)
                        {
                            Byte[] buffer = new Byte[client.Available];
                            // Read() blocks until data is available and throws exception
                            // if connection is closed.
                            client.GetStream().Read(buffer, 0, buffer.Length);
                            string chunk = Encoding.ASCII.GetString(buffer);

                            // Append the read data to the final result.
                            result += chunk;

                            // Check for end of message.
                            if (chunk.EndsWith("\r\n"))
                            {
                                // Remove trailing newline.
                                result = result.Remove(result.Length - 2, "\r\n".Length);
                                break;
                            }
                        }

                        Task.Run(() =>
                        {
                            CommunicationHandler.HandleMessage(this, new Message(result));
                        });
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
