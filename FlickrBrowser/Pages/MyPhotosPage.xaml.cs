using System.Collections.Generic;
using FlickrBrowser.DataModel;
using FlickrBrowser.Infrastructure;
using Windows.UI.Xaml;

namespace FlickrBrowser.Pages
{
    public sealed partial class MyPhotosPage
    {
        private IEnumerable<LibraryPhoto> _photos;
        private readonly PicturesLibraryManager _picturesLibraryManager;

        public MyPhotosPage()
        {
            InitializeComponent();

            // Would use IOC in a real-world app
            _picturesLibraryManager = new PicturesLibraryManager();

            BindEventHandlers();
        }


        private void BindEventHandlers()
        {
            AppBarButtonClicked += (o, e) =>
            {
                switch (e.AppBarCommand)
                {
                    case AppBarCommand.Refresh:
                        _photos = null;
                        PopulateGridView();
                        break;
                }
            };
        }


        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            ToggleLoadingAnimation(visible: true);
            PopulateGridView();
        }

        private async void PopulateGridView()
        {
            if (_photos == null)
                _photos = await _picturesLibraryManager.LoadFilesFromPictureLibraryAsync();

            DefaultViewModel["Photos"] = _photos;
            ToggleLoadingAnimation(visible: false);
        }

        private void headerUserControl_MenuItemClicked(object sender, Infrastructure.MenuItemClickedEventArgs e)
        {
            NavigationManager.NavigateToPage(e.TargetPage);
        }

        private void ToggleLoadingAnimation(bool visible)
        {
            progressLoading.IsActive = visible;
            progressLoading.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
