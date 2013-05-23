using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace FlickrBrowser.DataModel
{
    public class PicturesLibraryManager
    {
        private readonly BackgroundDownloader _backgroundDownloader;

        public PicturesLibraryManager()
        {
            _backgroundDownloader = new BackgroundDownloader();
        }

        public async Task<IEnumerable<LibraryPhoto>> LoadFilesFromPictureLibraryAsync()
        {
            var queryOptions = new QueryOptions(CommonFileQuery.OrderByDate, new[] { ".jpg", ".gif", ".png" });
            var query = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(queryOptions);

            var files = await query.GetFilesAsync();

            var photos = new List<LibraryPhoto>();
            foreach (var file in files)
            {
                photos.Add(new LibraryPhoto
                {
                    Title = file.DisplayName,
                    DateCreated = file.DateCreated.LocalDateTime,
                    Path = file.Path,
                    Thumbnail = await GetThumbnail(file)
                });
            }
            return photos;
        }

        public async Task SaveFlickrPhotoToPictureLibraryAsync(FlickrPhoto photo)
        {
            using (var client = new HttpClient())
            {
                var image = await client.GetByteArrayAsync(photo.LargeImageUrl);
                var localFile = await KnownFolders.PicturesLibrary.CreateFileAsync(GetFilenameFromUrl(photo.LargeImageUrl), CreationCollisionOption.GenerateUniqueName);

                using (var fileStream = await localFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var writeStream = fileStream.GetOutputStreamAt(0))
                    {
                        using (var writer = new DataWriter(writeStream))
                        {
                            writer.WriteBytes(image);
                            await writer.StoreAsync();
                            writer.DetachStream();
                        }
                        await writeStream.FlushAsync();
                    }
                }
            }
        }

        public async Task SaveBitmapToPictureLibrary(StorageFile image)
        {
            await image.CopyAsync(KnownFolders.PicturesLibrary);
        }

        private string GetFilenameFromUrl(string url)
        {
            var uri = new Uri(url);
            return uri.Segments[uri.Segments.Length - 1];
        }

        private async Task<StorageItemThumbnail> GetThumbnail(StorageFile file)
        {
            return await file.GetThumbnailAsync(ThumbnailMode.PicturesView, 230);
        }
    }
}
