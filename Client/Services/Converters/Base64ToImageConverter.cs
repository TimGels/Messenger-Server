using Messenger_Client.Models;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Messenger_Client.Services.Converters
{
    class Base64ToImageConverter : IValueConverter
    {
        /// <summary>
        /// Converting a base64 string to BitmapImage.
        /// </summary>
        /// <param name="value"></param>
        /// <returns><see cref="Task{BitmapImage}"/></returns>
        public async Task<BitmapImage> Convert(object value)
        {
            byte[] bytes = System.Convert.FromBase64String((string)value);
            BitmapImage image = null;
            try
            {
                await Helper.RunOnUIAsync(async () =>
                {
                    image = new BitmapImage();
                    InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
                    await stream.WriteAsync(bytes.AsBuffer());
                    stream.Seek(0);

                    await image.SetSourceAsync(stream);
                });
            }
            catch (Exception)
            {
                throw;
            }
            return image;
        }

        /// <summary>
        /// Needed for interface implementation. Does not do anything.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }

        /// <summary>
        /// Returns the Bitmap image converted from a base64 to the XAML.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var task = Task.Run(() => Convert((string)value));
            return new TaskCompletionNotifier<BitmapImage>(task);
        }
    }

    /// <summary>
    /// Notifies the caller that the task is complete.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class TaskCompletionNotifier<TResult> : INotifyPropertyChanged
    {
        public TaskCompletionNotifier(Task<TResult> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var scheduler = (SynchronizationContext.Current == null) ? TaskScheduler.Current : TaskScheduler.FromCurrentSynchronizationContext();
                task.ContinueWith(t =>
                {
                    var propertyChanged = PropertyChanged;
                    if (propertyChanged != null)
                    {
                        propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
                        if (t.IsCanceled)
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
                        }
                        else if (t.IsFaulted)
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                            propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
                        }
                        else
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                            propertyChanged(this, new PropertyChangedEventArgs("Result"));
                        }
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                scheduler);
            }
        }

        /// <summary>
        /// Gets the task being watched. This property never changes and is never <c>null</c>.
        /// </summary>
        public Task<TResult> Task { get; private set; }

        /// <summary>
        /// Gets the result of the task. Returns the default value of TResult if the task has not completed successfully.
        /// </summary>
        public TResult Result { get { return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult); } }

        /// <summary>
        /// Gets whether the task has completed.
        /// </summary>
        public bool IsCompleted { get { return Task.IsCompleted; } }

        /// <summary>
        /// Gets whether the task has completed successfully.
        /// </summary>
        public bool IsSuccessfullyCompleted { get { return Task.Status == TaskStatus.RanToCompletion; } }

        /// <summary>
        /// Gets whether the task has been canceled.
        /// </summary>
        public bool IsCanceled { get { return Task.IsCanceled; } }

        /// <summary>
        /// Gets whether the task has faulted.
        /// </summary>
        public bool IsFaulted { get { return Task.IsFaulted; } }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
