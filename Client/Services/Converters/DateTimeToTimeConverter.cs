using System;
using Windows.UI.Xaml.Data;

namespace Messenger_Client.Services.Converters
{
    class DateTimeToTimeConverter :IValueConverter
    {
        /// <summary>
        /// Convert DateTime object to a time string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns> <see cref="string"/></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dateTime = (DateTime)value;
            return dateTime.ToShortTimeString();
        }

        /// <summary>
        /// Needed for the interface. Currently not in use.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
