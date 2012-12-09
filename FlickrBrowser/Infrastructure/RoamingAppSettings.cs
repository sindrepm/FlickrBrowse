using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace FlickrBrowser.Infrastructure
{
    public class RoamingAppSettings
    {
        public static readonly RoamingAppSettings Instance = new RoamingAppSettings();
        private readonly ApplicationDataContainer _roamingSettings;

        private const string FlickrApiKeyKey = "FlickrApiKeyKey";
        private const string FlickrApiUrlKey = "FlickrApiUrlKey";
        private const string DefaultFlickrApiUrl = "http://api.flickr.com/services/rest/";

        private const string HighPriorityKey = "HighPriority";

        public string FlickrApiKey
        {
            get { return GetValue<string>(FlickrApiKeyKey); }
            set { _roamingSettings.Values[FlickrApiKeyKey] = value; }
        }

        public string FlickrApiUrl
        {
            get { return GetValue<string>(FlickrApiUrlKey); }
            set { _roamingSettings.Values[FlickrApiUrlKey] = value; }
        }

        public string HighPriority
        {
            get
            {
                return GetValue<string>(HighPriorityKey);
            }
            set
            {
                _roamingSettings.Values[HighPriorityKey] = value;
            }
        }

        private RoamingAppSettings()
        {
            _roamingSettings= ApplicationData.Current.RoamingSettings;
            InitDefaults();
        }

        private void InitDefaults()
        {
            if (string.IsNullOrEmpty(FlickrApiUrl))
                FlickrApiUrl = DefaultFlickrApiUrl;
        }

        private T GetValue<T>(string key)
        {
            if (_roamingSettings.Values.ContainsKey(key))
                return (T)_roamingSettings.Values[key];

            return default(T);
        }
    }
}
