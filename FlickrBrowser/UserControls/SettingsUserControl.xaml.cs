using FlickrBrowser.Infrastructure;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace FlickrBrowser.UserControls
{
    public sealed partial class SettingsUserControl : UserControl
    {
        public SettingsUserControl()
        {
            InitializeComponent();

            BindEventHandlers();
        }

        private void BindEventHandlers()
        {
            Loaded += SettingsUserControl_Loaded;
            Unloaded += SettingsUserControl_Unloaded;
        }

        void SettingsUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            LocalAppSettings.Instance.FlickrApiKey = txtApiKey.Text.Trim();
            LocalAppSettings.Instance.FlickrApiUrl = txtApiUrl.Text.Trim();
        }

        void SettingsUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtApiKey.Text = LocalAppSettings.Instance.FlickrApiKey.DefaultIfNullOrEmpty("<insert api key>");
            txtApiUrl.Text = LocalAppSettings.Instance.FlickrApiUrl.DefaultIfNullOrEmpty("<insert api url>");
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            var popup = Parent as Popup;
            if (popup != null)
                popup.IsOpen = false;

            SettingsPane.Show();
        }
    }
}