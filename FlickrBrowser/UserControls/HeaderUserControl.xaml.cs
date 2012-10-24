using System;
using System.Resources;
using FlickrBrowser.Infrastructure;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Linq;
using Windows.UI.Xaml.Media.Animation;
using System.Threading.Tasks;

namespace FlickrBrowser.UserControls
{
    public sealed partial class HeaderUserControl : UserControl
    {
        private int _activeMenuItemIndex = -1;

        public event EventHandler<MenuItemClickedEventArgs> MenuItemClicked;
        
        private static bool _isFirstTimeLoad = true;

        public int ActiveMenuItemIndex
        {
            get { return _activeMenuItemIndex; }
            set { _activeMenuItemIndex = value; }
        }

        public HeaderUserControl()
        {
            InitializeComponent();

            Loaded += HeaderUserControl_Loaded;
            SizeChanged += HeaderUserControl_SizeChanged;
        }

        void HeaderUserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdaptToViewState();
        }

        async void HeaderUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetActiveMenuItem();

            if (_isFirstTimeLoad)
            {
                _isFirstTimeLoad = false;
                await Task.Delay(TimeSpan.FromSeconds(2));
                AnimateLogo();
            }
        }

        private void AdaptToViewState()
        {
            if (ApplicationView.Value == ApplicationViewState.Snapped)
            {
                pageTitle1.Style = pageTitle2.Style = App.Current.Resources["PageSubheaderTextStyle"] as Style;
                flickrLogo.Width = 20;
                flickrLogo.Margin = new Thickness(5, 18, 10, 0);
                foreach (var button in menuItemsPanel.Children.OfType<Button>())
                    button.Style = App.Current.Resources["SnappedMenuItemButtonStyle"] as Style;

                menuItemsPanel.Orientation = Orientation.Vertical;
            }
            else
            {
                pageTitle1.Style = pageTitle2.Style = App.Current.Resources["PageHeaderTextStyle"] as Style;
                flickrLogo.Width = 40;
                flickrLogo.Margin = new Thickness(5, 0, 15, 0);
                foreach (var button in menuItemsPanel.Children.OfType<Button>())
                    button.Style = App.Current.Resources["MenuItemButtonStyle"] as Style;

                menuItemsPanel.Orientation = Orientation.Horizontal;
            }
        }

        private void SetActiveMenuItem()
        {
            var menuItems = menuItemsPanel.Children.OfType<Button>().ToList();
            if (ActiveMenuItemIndex >= 0 && ActiveMenuItemIndex < menuItems.Count)
                menuItems[ActiveMenuItemIndex].Foreground =
                    Application.Current.Resources["ApplicationPointerOverForegroundThemeBrush"] as Brush;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            OnMenuItemClicked(new MenuItemClickedEventArgs(TargetPage.Home));
        }

        private void btnRecentlyAdded_Click(object sender, RoutedEventArgs e)
        {
            OnMenuItemClicked(new MenuItemClickedEventArgs(TargetPage.RecentlyAdded));
        }

        private void btnMyPhotos_Click(object sender, RoutedEventArgs e)
        {
            OnMenuItemClicked(new MenuItemClickedEventArgs(TargetPage.MyPhotos));
        }

        private void OnMenuItemClicked(MenuItemClickedEventArgs e)
        {
            var handler = MenuItemClicked;

            if (handler != null)
                handler(this, e);
        }

        private void flickrLogo_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            AnimateLogo();
        }

        private void AnimateLogo()
        {
            var storyboard = new Storyboard();
            var doubleAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(2)),
                RepeatBehavior = new RepeatBehavior(1),
                EasingFunction = new BounceEase()
            };
            storyboard.Children.Add(doubleAnimation);
            Storyboard.SetTarget(doubleAnimation, flickrLogo);
            Storyboard.SetTargetProperty(doubleAnimation, "(Image.RenderTransform).(RotateTransform.Angle)");

            storyboard.Begin();
        }
    }
}
