using FlickrBrowser.Pages;
using Windows.UI.Xaml.Controls;

namespace FlickrBrowser.Infrastructure
{
    public class NavigationManager
    {
        private readonly Frame _frame;

        public NavigationManager(Frame frame)
        {
            _frame = frame;
        }

        public void NavigateToPage(TargetPage page)
        {
            switch (page)
            {
                case TargetPage.Home:
                    _frame.Navigate(typeof(HomePage));
                    break;
                case TargetPage.RecentlyAdded:
                    _frame.Navigate(typeof(RecentlyAddedPage));
                    break;
                case TargetPage.MyPhotos:
                    _frame.Navigate(typeof(MyPhotosPage));
                    break;
            }
        }

        public void GoBack()
        {
            if (_frame.CanGoBack)
                _frame.GoBack();
        }

        public void GoForward()
        {
            if (_frame.CanGoForward)
                _frame.GoForward();
        }
    }
}
