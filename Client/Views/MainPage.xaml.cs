using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UWP_Messenger_Client.Models;
using Windows.UI.Xaml.Controls;

namespace UWP_Messenger_Client.Views
{
    public sealed partial class MainPage : Page
    {
        Client Client;

        /// <summary>
        /// for testing purposes
        /// </summary>
        Group group;

        public MainPage()
        {
            this.InitializeComponent();
            this.Client = Client.Instance;

            //for testing -> create group and send every 3 seconds a message.
            this.group = new Group("eerste Groep", 1);

            Thread writerThread = new Thread(writerDoWork);
            writerThread.Start();
        }

        private void writerDoWork()
        {
            while (true)
            {
                group.SendMessage("Hello from UWP!");
                Thread.Sleep(3000);
            }
        }

    }
}
