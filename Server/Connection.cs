using Shared;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            lock(closedLock)
            {
                if (!closed)
                {
                    Server.Instance.DeleteConnection(this);
                    client.Close();
                    client.Dispose();
                    closed = true;
                }
            }
        }

        public override void ReadData()
        {
            try
            {
                while (true)
                {
                    Byte[] buffer = new Byte[client.Available];
                    // Read() blocks until data is available and throws exception if
                    // connection is closed.
                    client.GetStream().Read(buffer, 0, buffer.Length);
                    string data = Encoding.ASCII.GetString(buffer);

                    // HACK: Should use DataAvaliable. On every message from the client,
                    // another one is send with an empty string.
                    if (!data.Equals(""))
                    {
                        Task.Run(() => CommunicationHandler.HandleMessage(this, new Message(data)));
                    }

                    Thread.Sleep(50);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Exception of type {0} occured in {1}",
                    e.GetType().FullName,
                    MethodBase.GetCurrentMethod().Name));
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
            try
            {
                base.SendData(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Exception of type {0} occured in {1}",
                    e.GetType().FullName,
                    MethodBase.GetCurrentMethod().Name));
            }
        }
    }
}
