using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
            while (true)
            {
                try
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
                            CommunicationHandler.HandleMessage(new Message(result));
                        });
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    break;
                }
            }
        }
    }
}
