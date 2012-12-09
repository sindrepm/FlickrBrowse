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
            
            
            if (btnRoamingSettings.IsChecked.GetValueOrDefault())
            {
                LocalAppSettings.Instance.UseRoaming = true;
                RoamingAppSettings.Instance.FlickrApiKey = txtApiKey.Text.Trim();
                RoamingAppSettings.Instance.FlickrApiUrl = txtApiUrl.Text.Trim();
            }
            if (btnCategorySettings.IsChecked.GetValueOrDefault())
            {
                LocalAppSettings.Instance.RememberCategory = true;
            }
            
        }

        void SettingsUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtApiKey.Text = LocalAppSettings.Instance.FlickrApiKey.DefaultIfNullOrEmpty("<insert api key>");
            txtApiUrl.Text = LocalAppSettings.Instance.FlickrApiUrl.DefaultIfNullOrEmpty("<insert api url>");
            
            if (LocalAppSettings.Instance.UseRoaming)
            {
                txtApiKey.Text = RoamingAppSettings.Instance.FlickrApiKey.DefaultIfNullOrEmpty("<insert api key>");
                txtApiUrl.Text = RoamingAppSettings.Instance.FlickrApiUrl.DefaultIfNullOrEmpty("<insert api url>");
                btnRoamingSettings.IsChecked = true;
            }
            if (LocalAppSettings.Instance.RememberCategory)
            {
                btnCategorySettings.IsChecked = true;
            }
            
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            var popup = Parent as Popup;
            if (popup != null)
                popup.IsOpen = false;

            SettingsPane.Show();
        }

       
        private void btnRoamingSettings_Checked(object sender, RoutedEventArgs e)
        {
            RoamingAppSettings.Instance.FlickrApiKey = txtApiKey.Text.Trim();
            RoamingAppSettings.Instance.FlickrApiUrl = txtApiUrl.Text.Trim();
            LocalAppSettings.Instance.UseRoaming = true;
        }

        private void btnRoamingSettings_UnChecked(object sender, RoutedEventArgs e)
        {
            RoamingAppSettings.Instance.FlickrApiKey = null;
            RoamingAppSettings.Instance.FlickrApiUrl = null;
            LocalAppSettings.Instance.UseRoaming = false;
            RoamingAppSettings.Instance.HighPriority = null;
        }

        private void btnCategorySettings_Checked(object sender, RoutedEventArgs e)
        {
            if (!btnRoamingSettings.IsChecked.GetValueOrDefault())
            {
                RoamingAppSettings.Instance.HighPriority = null; 
            }
        }

        private void btnCategorySettings_UnChecked(object sender, RoutedEventArgs e)
        {
            RoamingAppSettings.Instance.HighPriority = null;
            LocalAppSettings.Instance.RememberCategory = false;
        }
    }
}