using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlickrBrowser.Infrastructure;
using MesanApplicationFramework.Common.Communication;

namespace FlickrBrowser.DataModel
{
    public sealed class FlickrApi
    {
        private readonly string _apiKey;

        public static readonly FlickrApi Instance = new FlickrApi();
        private readonly RestClient _restClient;

        public event EventHandler<ApiErrorOccuredEventArgs> ApiErrorOccured;

        private FlickrApi()
        {
            _restClient = new RestClient(LocalAppSettings.Instance.FlickrApiUrl);
            _restClient.ResponseConverter = new DefaultXmlRestResponseConverter();
            _apiKey = LocalAppSettings.Instance.FlickrApiKey;
        }

        public async Task<IEnumerable<FlickrPhoto>> GetRecentlyAddedPhotosAsync(int? numberOfPhotos = 100)
        {
            IEnumerable<FlickrPhoto> flickrPhotos = new FlickrPhoto[0];
            try
            {
                var command = new GetRecentFlickrPhotosCommand(_apiKey);
                command.AddParameter("per_page", (numberOfPhotos ?? 100).ToString());
                command.AddParameter("extras", "owner_name,date_upload,original_format,date_taken");

                var result = await _restClient.ExecuteAsync(command);
                flickrPhotos = FlickrHelpers.CreateFlickrPhotoFromXml(result.Data.Root);
            }
            catch (Exception e)
            {
                OnApiErrorOccured(new ApiErrorOccuredEventArgs(e) { ErrorMessage = e.Message });
            }
            return flickrPhotos;
        }

        public void OnApiErrorOccured(ApiErrorOccuredEventArgs e)
        {
            var handler = ApiErrorOccured;
            if (handler != null)
                handler(this, e);
        }
    }
}
