using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlickrBrowser.DataModel;
using FlickrBrowser.Infrastructure;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace FlickrBrowser.Pages
{
    public sealed partial class PhotoRecievedPage
    {
        public PhotoRecievedPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var share = e.Parameter as ShareOperation;

            if (share != null)
            {
                if (share.Data.Contains(StandardDataFormats.StorageItems))
                {
                    var storageItems = await share.Data.GetStorageItemsAsync();

                    if (storageItems.Count > 0 && storageItems[0] is StorageFile)
                    {
                        var item = (storageItems[0] as StorageFile);

                        photoRecievedContainer.ImageFailed += 
                            (sender, args) =>
                        {
                            lblImagePath.Text = args.ErrorMessage;
                        };

                        photoRecievedContainer.ImageOpened += 
                            (sender, args) =>
                        {
                            lblImagePath.Text = item.DisplayName;
                        };

                        photoRecievedContainer.Source = await GetBitmapImageAsync(item);
                    }
                }
            }
        }

        /// <summary>
        /// Metode for å gjøre om fil til bilde.
        /// </summary>
        private async Task<BitmapImage> GetBitmapImageAsync(StorageFile storageFile)
        {
            var bitmapImage = new BitmapImage();
            IAsyncOperation<IRandomAccessStream> operation = storageFile.OpenAsync(FileAccessMode.Read);
            IRandomAccessStream stream = await operation;
            bitmapImage.SetSource(stream);
            return bitmapImage;
        }
    }
}
