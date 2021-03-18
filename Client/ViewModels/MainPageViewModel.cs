using Messenger_Client.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Chat;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Messenger_Client.ViewModels
{
    public class MainPageViewModel
    {
        public List<TestMessage> MessageList { get; set; }

        //public Client Client { get; set; }
        public List<Group> GroupList { get; set; }

        public MainPageViewModel()
        {
            this.MessageList = new List<TestMessage>();
            this.GroupList = new List<Group>();


            Message message = new Message()
            {
                ClientName = "ClientName1",
                ClientId = 1,
                DateTime = DateTime.Now,
                GroupID = 2,
                MessageType = MessageType.ChatMessage,
            };


            //Image image = new Image();

            Task.Run(async () =>
            {
                image.sou getfile();
            });

            //for (int i = 0; i < 30; i++)
            //{

            //    this.MessageList.Add(new TestMessage());
            //}



            //Client client = Client.Instance;

            //Message message = new Message()
            //{
            //    ClientName = "ClientName1",
            //    ClientId = 1,
            //    DateTime = DateTime.Now,
            //    GroupID = 2,
            //    MessageType = MessageType.ChatMessage,
            //};
            //message.Image = new Bitmap(new Uri("ms-appx:///Assets/StoreLogo.png"));

            ////Bitmap bitmapImage = new Bitmap(new Uri("ms-appx:///[project-name]/Assets/image.jpg"));
            //StorageFile file = StorageFile.GetFileFromPathAsync("ms-appx:///Assets/StoreLogo.png").AsTask().Result;
            //Image image = Image.FromFile("ms-appx:///Assets/StoreLogo.png");
            //message.Image = image;

            //Image image = Image.FromHbitmap(bitmapImage);


            //for (int i = 0; i < 20; i++)
            //{
            //    Group group = new Group("GroupName", i);
            //    client.AddGroup(group);
            //    GroupList.Add(group);

            //    for (int j = 0; j < 10; j++)
            //    {
            //        group.AddMessage(message);
            //    }

            //}
        }

        private async Task<BitmapImage> getfile()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/StoreLogo.png"));
            //Image image = Image.FromStream(string.);


            //BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));



            //Windows.Storage.Streams.Buffer buf = (Windows.Storage.Streams.Buffer)await FileIO.ReadBufferAsync(file);

            // Ensure the stream is disposed once the image is loaded
            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                // Set the image source to the selected bitmap
                BitmapImage bitmapImage = new BitmapImage();
                // Decode pixel sizes are optional
                // It's generally a good optimisation to decode to match the size you'll display
                //bitmapImage.DecodePixelHeight = decodePixelHeight;
                //bitmapImage.DecodePixelWidth = decodePixelWidth;

                await bitmapImage.SetSourceAsync(fileStream);

                return bitmapImage;

                //mypic.Source = bitmapImage;
                
            }

            //Debug.WriteLine(buf.ToString());
        }
    }
}
