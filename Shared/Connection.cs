using System;
using System.Net.Sockets;
using System.Text;

namespace Shared
{
    public abstract class Connection
    {
        /// <summary>
        /// The client for which to send and receive data.
        /// </summary>
        protected readonly TcpClient client;

        /// <summary>
        /// Synchronises access to the Send() buffer.
        /// </summary>
        private readonly object writeLocker = new object();

        public Connection(TcpClient client)
        {
            this.client = client;
        }

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
            string data = Message.SerializeMessage(message) + "\r\n";
            Byte[] buffer = Encoding.ASCII.GetBytes(data);
            lock (writeLocker)
            {
                client.GetStream().Write(buffer, 0, buffer.Length);
            }
        }
    }
}
