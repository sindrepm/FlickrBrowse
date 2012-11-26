using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlickrBrowser.DataModel;
using FlickrBrowser.Infrastructure;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
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
                    IReadOnlyList<IStorageItem> storageItems = await share.Data.GetStorageItemsAsync();
                    var fileName = storageItems[0].Name;

                    StorageFolder storageFolder = KnownFolders.PicturesLibrary;
                    StorageFile sampleFile = await storageFolder.GetFileAsync(fileName);

                    //lblImagePath.Text = sampleFile.Path;

                    photoRecievedContainer.ImageFailed += (sender, args) =>
                                                              {
                                                                  var error = args.ErrorMessage;
                                                                  lblImagePath.Text = error;
                                                              };

                    photoRecievedContainer.Source = await GetBitmapImageAsync(sampleFile);
                }
            }
        }

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
