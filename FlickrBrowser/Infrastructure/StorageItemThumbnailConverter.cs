using System;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace FlickrBrowser.Infrastructure
{
    public class StorageItemThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value.GetType() != typeof(StorageItemThumbnail))
            {
                throw new ArgumentException("Expected a thumbnail");
            }
            if (targetType != typeof(ImageSource))
            {
                throw new ArgumentException("What are you trying to convert to here?");
            }

            StorageItemThumbnail thumbnail = (StorageItemThumbnail)value;
            BitmapImage image = new BitmapImage();
            image.SetSource(thumbnail);
            return (image);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }  
    }
}
