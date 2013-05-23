using System;
using System.Collections.Generic;
using System.Linq;
using FlickrBrowser.DataModel;
using FlickrBrowser.Infrastructure;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Search;

namespace FlickrBrowser.Pages
{
    public sealed partial class HomePage
    {
        private static IEnumerable<FlickrPhoto> _photos;
        private readonly PicturesLibraryManager _picturesLibraryManager;
        private DataTransferManager _dataTransferManager;

        public HomePage()
        {
            this.InitializeComponent();

            BindEventHandlers();

            DefaultViewModel["IsDownloadEnabled"] = true;

            // Would use IOC in a real-world app
            _picturesLibraryManager = new PicturesLibraryManager();
            _dataTransferManager = DataTransferManager.GetForCurrentView();
        }

        private void BindEventHandlers()
        {
            AppBarButtonClicked += (o, e) =>
            {
                switch (e.AppBarCommand)
                {
                    case AppBarCommand.Refresh:
                        _photos = null;
                        GetPhotosAsync();
                        break;
                    case AppBarCommand.Download:
                        SaveSelectedPhoto();
                        break;
                }
            };

            SizeChanged += HomePage_SizeChanged;

            ShowSearchPaneOnKeyboardInput();
        }

        private static void ShowSearchPaneOnKeyboardInput()
        {
            var searchPane = SearchPane.GetForCurrentView();
            if (searchPane != null)
                searchPane.ShowOnKeyboardInput = true;
        }

        void HomePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ApplicationView.Value == ApplicationViewState.Snapped)
                ToggleLoadingAnimation(visible: false);
        }

        private async void SaveSelectedPhoto()
        {
            var selectedPhoto = flipView.SelectedItem as FlickrPhoto;
            if (selectedPhoto == null)
            {
                ShowMessage("No image selected!");
                return;
            }

            await _picturesLibraryManager.SaveFlickrPhotoToPictureLibraryAsync(selectedPhoto);
            ShowMessage("Image was saved to Pictures Library");
        }

        private async void ShowMessage(string message)
        {
            var msg = new MessageDialog(message);
            await msg.ShowAsync();
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (string.IsNullOrEmpty(LocalAppSettings.Instance.FlickrApiKey))
                ShowApiKeyWarning();

            GetPhotosAsync();
           
            // Del bilder etc med omverdenen...
            _dataTransferManager.DataRequested += DataTransferManagerOnDataRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _dataTransferManager.DataRequested -= DataTransferManagerOnDataRequested;
        }

        private async void DataTransferManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataPackage requestData = args.Request.Data;
            requestData.Properties.Title = "FlickrShare";
            requestData.SetText("FlickShare shares its sharings");

            if (_photos != null)
            {
                var photoToShare = _photos.First();

                requestData.Properties.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(photoToShare.ThumbnailUrl));
                requestData.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(photoToShare.LargeImageUrl)));
            }
            else
            {
                requestData.Properties.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri("http://ap.mnocdn.no/incoming/article7065505.ece/ALTERNATES/w680cFree/afp000443990.jpg?updated=111220120643"));
                requestData.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri("http://ap.mnocdn.no/incoming/article7065505.ece/ALTERNATES/w680cFree/afp000443990.jpg?updated=111220120643")));
                requestData.SetUri(new Uri("http://ap.mnocdn.no/incoming/article7066904.ece/ALTERNATES/w780c169/sxb5de30.jpg?updated=111220120638"));
            }
        }

        private async void ShowApiKeyWarning()
        {
            await new MessageDialog("Please enter a valid Flickr API key by going to Settings -> Flickr Settings. Restart the application for the changes to take effect.").ShowAsync();
        }

        private async void GetPhotosAsync()
        {
            if(_photos == null)
                _photos = await FlickrApi.Instance.GetRecentlyAddedPhotosAsync();

            DefaultViewModel["Photos"] = _photos;
        }

        private void headerUserControl_MenuItemClicked(object sender, Infrastructure.MenuItemClickedEventArgs e)
        {
            NavigationManager.NavigateToPage(e.TargetPage);
        }


        private void FlipView_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 0)
                return;

            ToggleLoadingAnimation(visible: true);

            var currentItem = flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem) as FlipViewItem;
            if (currentItem != null)
            {
                Storyboard sb = new Storyboard();
                DoubleAnimation da = new DoubleAnimation();
                da.From = 1.0;
                da.To = 0.0;
                da.AutoReverse = true;
                Duration dur = new Duration(TimeSpan.FromMilliseconds(500));
                da.Duration = dur;
                sb.Children.Add(da);
                Storyboard.SetTargetProperty(sb, "Opacity");
                Storyboard.SetTarget(sb, currentItem);
                sb.Begin();
            }

        }

        private void ToggleLoadingAnimation(bool visible)
        {
            progressLoading.IsActive = visible;
            progressLoading.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void flipView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (e.OriginalSource is Image)
                GlobalAppBar.IsOpen = true;
        }
    }
}
