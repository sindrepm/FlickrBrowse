using System;
using System.Linq;
using FlickrBrowser.Common;
using FlickrBrowser.Infrastructure;
using FlickrBrowser.UserControls;
using Windows.Foundation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace FlickrBrowser.Pages
{
    public partial class BasePage : LayoutAwarePage
    {
        private Popup _settingsPopup;
        private Rect _windowBounds = Window.Current.Bounds;
        private int _settingsWidth = 346;
        protected AppBar GlobalAppBar { get { return defaultAppBar; } }
        protected NavigationManager NavigationManager { get; private set; }

        protected event EventHandler<AppBarButtonClicked> AppBarButtonClicked;

        public BasePage()
        {
            InitializeComponent();

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                InitNavigationManager();
                DefaultViewModel["IsDownloadEnabled"] = false;
            }

            BindEventHandlers();
        }

        private void InitNavigationManager()
        {
            var frame = Window.Current.Content as Frame;
            if (frame != null)
                NavigationManager = new NavigationManager(Window.Current.Content as Frame);
        }

        private void BindEventHandlers()
        {
            SettingsPane.GetForCurrentView().CommandsRequested += BasePage_CommandsRequested;
            appBarBtnDownload.Click += appBarBtnDownload_Click;
            appBarBtnRefresh.Click += appBarBtnRefresh_Click;
        }

        void BasePage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            if (args.Request.ApplicationCommands.Any(o => o.Id.ToString() == "FlickrSettings"))
                return;

            var cmd = new SettingsCommand("FlickrSettings", "Flickr Settings", ShowFlickrSettings);
            args.Request.ApplicationCommands.Add(cmd);
        }

        private void ShowFlickrSettings(IUICommand command)
        {
            _settingsPopup = new Popup();
            _settingsPopup.ChildTransitions = new TransitionCollection { new EntranceThemeTransition { FromHorizontalOffset = 100 } };
            _settingsPopup.Closed += (o, e) => Window.Current.Activated -= OnWindowActivated;
            Window.Current.Activated += OnWindowActivated;
            _settingsPopup.IsLightDismissEnabled = true;
            _settingsPopup.Width = _settingsWidth;
            _settingsPopup.Height = _windowBounds.Height;
            
            var settingsUserControl = new SettingsUserControl
            {
                Width = _settingsWidth,
                Height = _windowBounds.Height
            };
            _settingsPopup.Child = settingsUserControl;
            _settingsPopup.SetValue(Canvas.LeftProperty, _windowBounds.Width - _settingsWidth);
            _settingsPopup.SetValue(Canvas.TopProperty, 0);
            _settingsPopup.IsOpen = true;
        }

        private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
                _settingsPopup.IsOpen = false;
        }

        private void appBarBtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            OnAppBarButtonClicked(new AppBarButtonClicked { AppBarCommand = AppBarCommand.Refresh });
        }

        private void appBarBtnDownload_Click(object sender, RoutedEventArgs e)
        {
            OnAppBarButtonClicked(new AppBarButtonClicked { AppBarCommand = AppBarCommand.Download });
        }

        public void OnAppBarButtonClicked(AppBarButtonClicked e)
        {
            var handler = AppBarButtonClicked;

            if (handler != null) 
                handler(this, e);
        }
    }
}