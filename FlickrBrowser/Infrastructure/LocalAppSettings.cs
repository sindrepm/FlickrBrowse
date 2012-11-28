using System;
using System.Linq.Expressions;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace FlickrBrowser.Infrastructure
{
    public class LocalAppSettings
    {
        public static readonly LocalAppSettings Instance = new LocalAppSettings();
        private readonly ApplicationDataContainer _localSettings;

        private const string FlickrApiKeyKey = "FlickrApiKeyKey";
        private const string FlickrApiUrlKey = "FlickrApiUrlKey";
        private const string DefaultFlickrApiUrl = "http://api.flickr.com/services/rest/";

        public string FlickrApiKey
        {
            get { return GetValue<string>(FlickrApiKeyKey); }
            set { _localSettings.Values[FlickrApiKeyKey] = value; }
        }

        public string FlickrApiUrl
        {
            get { return GetValue<string>(FlickrApiUrlKey); }
            set { _localSettings.Values[FlickrApiUrlKey] = value; }
        }

        private LocalAppSettings()
        {
            _localSettings= ApplicationData.Current.LocalSettings;

            InitDefaults();
        }

        private void InitDefaults()
        {
            if (string.IsNullOrEmpty(FlickrApiUrl))
                FlickrApiUrl = DefaultFlickrApiUrl;
        }

        private T GetValue<T>(string key)
        {
            if (_localSettings.Values.ContainsKey(key))
                return (T)_localSettings.Values[key];

            return default(T);
        }
    }
}
