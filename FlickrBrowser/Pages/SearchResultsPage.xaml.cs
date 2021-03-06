﻿using FlickrBrowser.Common;
using FlickrBrowser.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlickrBrowser.Infrastructure;
using Windows.ApplicationModel.Search;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace FlickrBrowser.Pages
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class SearchResultsPage : BasePage
    {
        private readonly PicturesLibraryManager _picturesLibraryManager;

        public SearchResultsPage()
        {
            this.InitializeComponent();

            BindEventHandlers();

            DefaultViewModel["IsDownloadEnabled"] = true;
            ShowSearchPaneOnKeyboardInput();

            _picturesLibraryManager = new PicturesLibraryManager();
        }

        private void BindEventHandlers()
        {
            AppBarButtonClicked += (o, e) =>
            {
                switch (e.AppBarCommand)
                {
                    case AppBarCommand.Download:
                        SaveSelectedPhoto();
                        break;
                }
            };
        }


        private static void ShowSearchPaneOnKeyboardInput()
        {
            var searchPane = SearchPane.GetForCurrentView();
            if (searchPane != null)
                searchPane.ShowOnKeyboardInput = true;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var queryText = navigationParameter as String;

            // TODO: Application-specific searching logic.  The search process is responsible for
            //       creating a list of user-selectable result categories:
            //
            //       filterList.Add(new Filter("<filter name>", <result count>));
            //
            //       Only the first filter, typically "All", should pass true as a third argument in
            //       order to start in an active state.  Results for the active filter are provided
            //       in Filter_SelectionChanged below.

            var filterList = new List<SearchFilter>();
            filterList.Add(new SearchFilter("All", 0, true));
            filterList.Add(new SearchFilter("Technology", 15));

            // Communicate results through the view model
            this.DefaultViewModel["QueryText"] = '\u201c' + queryText + '\u201d';
            this.DefaultViewModel["Filters"] = filterList;
            this.DefaultViewModel["ShowFilters"] = filterList.Count > 1;
        }

        /// <summary>
        /// Invoked when a filter is selected using the ComboBox in snapped view state.
        /// </summary>
        /// <param name="sender">The ComboBox instance.</param>
        /// <param name="e">Event data describing how the selected filter was changed.</param>
        async void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determine what filter was selected
            var selectedFilter = e.AddedItems.FirstOrDefault() as SearchFilter;
            if (selectedFilter != null)
            {
                // Mirror the results into the corresponding Filter object to allow the
                // RadioButton representation used when not snapped to reflect the change
                selectedFilter.Active = true;

                // TODO: Respond to the change in active filter by setting this.DefaultViewModel["Results"]
                //       to a collection of items with bindable Image, Title, Subtitle, and Description properties

                var searchQuery = DefaultViewModel["QueryText"] as string;
                var searchResults = await GetSearchResults(selectedFilter, searchQuery);

                (DefaultViewModel["Filters"] as ICollection<SearchFilter>).First(o => o.Name == "All").Count = searchResults.Count;

                if (searchResults.Count > 0)
                {
                    DefaultViewModel["Results"] = searchResults;
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                    return;
                }
            }

            // Display informational text when there are no search results.
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        private async Task<ICollection<FlickrPhoto>> GetSearchResults(SearchFilter filter, string query)
        {
            var strippedQuery = query.Trim('\u201c', '\u201d');
            if (string.IsNullOrEmpty(strippedQuery))
                return new List<FlickrPhoto>();

            // @todo: get photos based on filter and query string

           var result = await FlickrApi.Instance.GetRecentlyAddedPhotosAsync();
           return result.ToList();
        }

        /// <summary>
        /// Invoked when a filter is selected using a RadioButton when notc snapped.
        /// </summary>
        /// <param name="sender">The selected RadioButton instance.</param>
        /// <param name="e">Event data describing how the RadioButton was selected.</param>
        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            // Mirror the change into the CollectionViewSource used by the corresponding ComboBox
            // to ensure that the change is reflected when snapped
            if (filtersViewSource.View != null)
            {
                var filter = (sender as FrameworkElement).DataContext;
                filtersViewSource.View.MoveCurrentTo(filter);
            }
        }

        private void headerUserControl_MenuItemClicked(object sender, Infrastructure.MenuItemClickedEventArgs e)
        {
            NavigationManager.NavigateToPage(e.TargetPage);
        }

        private async void SaveSelectedPhoto()
        {
            var selectedPhoto = resultsGridView.SelectedItem as FlickrPhoto;
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
    }
}
