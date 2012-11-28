using Windows.Storage.FileProperties;

namespace FlickrBrowser.DataModel
{
    public class LibraryPhoto : Photo
    {
        public string Path { get; set; }
        public StorageItemThumbnail Thumbnail { get; set; }
    }
}
