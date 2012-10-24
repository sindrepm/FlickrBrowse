using System.Xml.Linq;
using MesanMobilityFramework.Common.Communication;

namespace FlickrBrowser.DataModel
{
    public class GetRecentFlickrPhotosCommand : RestCommand<XDocument>
    {
        public GetRecentFlickrPhotosCommand(string apiKey)
        {
            AddParameter("method", "flickr.photos.getRecent");
            AddParameter("api_key", apiKey);
            AddParameter("format", "rest");
        }
    }
}
