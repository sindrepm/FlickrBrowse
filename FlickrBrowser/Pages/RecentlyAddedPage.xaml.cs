using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlickrBrowser.DataModel;
using FlickrBrowser.Infrastructure;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FlickrBrowser.Pages
{
    public sealed partial class RecentlyAddedPage
    {
        private static IEnumerable<FlickrPhoto> _photos;
        private readonly PicturesLibraryManager _picturesLibraryManager;

        public RecentlyAddedPage()
        {
            InitializeComponent();

            BindEventHandlers();

            DefaultViewModel["IsDownloadEnabled"] = true;

            // Would use IOC in a real-world app
            _picturesLibraryManager = new PicturesLibraryManager();
        }

        private void BindEventHandlers()
        {
            AppBarButtonClicked += (o, e) =>
            {
                switch (e.AppBarCommand)
                {
                    case AppBarCommand.Refresh:
                        RefreshGridView();
                        break;
                    case AppBarCommand.Download:
                        SaveSelectedPhoto();
                        break;
                }
            };
        }

        private async void SaveSelectedPhoto()
        {
            var selectedPhoto = imagesGridView.SelectedItem as FlickrPhoto;
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

        private void RefreshGridView()
        {
            _photos = null;
            PopulateGridViewAsync();
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            ToggleLoadingAnimation(visible: true);
            PopulateGridViewAsync();
        }

        private async void PopulateGridViewAsync()
        {
            if(_photos == null)
                _photos = await LoadRecentlyAddedPhotosAsync();

            DefaultViewModel["Photos"] = _photos;
            ToggleLoadingAnimation(visible: false);
        }

        private void SelectedImageChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalAppBar.IsOpen = true;
        }

        private void headerUserControl_MenuItemClicked(object sender, MenuItemClickedEventArgs e)
        {
            NavigationManager.NavigateToPage(e.TargetPage);
        }

        private async Task<IEnumerable<FlickrPhoto>> LoadRecentlyAddedPhotosAsync()
        {
            return await FlickrApi.Instance.GetRecentlyAddedPhotosAsync();
        }

        private void ToggleLoadingAnimation(bool visible)
        {
            progressLoading.IsActive = visible;
            progressLoading.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}