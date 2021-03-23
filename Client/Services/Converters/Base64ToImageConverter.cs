

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Messenger_Client.Services.Converters
{
    class Base64ToImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string s = value as string;

            if (s == null)
                return null;

            BitmapImage bi = new BitmapImage();

            //bi.BeginInit();
            //bi.StreamSource = new MemoryStream(System.Convert.FromBase64String(s));
            //bi.EndInit();

            //var bytes = System.Convert.FromBase64String(s);
            //var contents = new System.Net.Http.StreamContent(new MemoryStream(bytes));
            //bi.SetSource(contents);

            //bi.SetSource(new StreamContent(System.Convert.FromBase64String(bytes)));

            //Task.Run(() =>
            //{
            //    byte[] byteBuffer = System.Convert.FromBase64String(s);
            //    MemoryStream memoryStream = new MemoryStream(byteBuffer);
            //    memoryStream.Position = 0;

            //    BitmapImage bitmapImage = new BitmapImage();
            //    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());

            //    memoryStream.Close();
            //    memoryStream = null;
            //    byteBuffer = null;

            //    return bitmapImage;
            //});
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}

