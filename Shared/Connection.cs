using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Shared
{
    public abstract class Connection
    {
        /// <summary>
        /// Used to cancel the Read() call.
        /// </summary>
        protected CancellationTokenSource readerCts;

        /// <summary>
        /// Indicates if the current connection has been closed already.
        /// </summary>
        protected bool closed = false;

        /// <summary>
        /// Since Close() can be called from the Read() thread or the CommuncationHandler
        /// thread, this lock is used to make sure the Close() only executes once.
        /// </summary>
        protected readonly object closedLock = new object();

        /// <summary>
        /// The client for which to send and receive data.
        /// </summary>
        protected TcpClient client;

        /// <summary>
        /// Synchronises access to the Send() buffer.
        /// </summary>
        private readonly object writeLock = new object();

        /// <summary>
        /// Base connection constructor.
        /// </summary>
        /// <param name="client">The TcpClient to instantiate this connection with.</param>
        public Connection(TcpClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Closes the connection. Should only take effect once.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Continuously try to read data from the stream. Any incoming message is
        /// handed to a seperate Task, which is responsible for handling the message.
        /// </summary>
        public abstract void ReadData();

        /// <summary>
        /// Send a message to this client.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public virtual void SendData(Message message)
        {
            // Allocate a buffer large enough to hold the data and length prefix.
            string data = Message.SerializeMessage(message);
            Byte[] buffer = new byte[sizeof(Int32) + data.Length];

            // Convert the data length to bytes and write to the first 4 bytes before
            // reading the data in the leftover space.
            Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, buffer, 0, sizeof(Int32));
            Encoding.ASCII.GetBytes(data);
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(data), 0, buffer, sizeof(Int32), data.Length);

            // Synchronise Send() and set the SendBufferSize.
            lock (writeLock)
            {
                client.SendBufferSize = buffer.Length;
                client.GetStream().Write(buffer, 0, buffer.Length);
            }
        }
    }
}
