using System;

namespace FlickrBrowser.DataModel
{
    public class FlickrPhoto : Photo
    {
        public enum ImageSize
        {
            Thumbnail100,
            Small240,
            Small320,
            Large1024,
            Original
        }

        public string Id { get; set; }
        public string Owner { get; set; }
        public string OwnerName { get; set; }
        public string Secret { get; set; }
        public string OriginalSecret { get; set; }
        public string ServerId { get; set; }
        public string FarmId { get; set; }
        public string DateTaken { get; set; }
        public string OriginalFormat { get; set; }
        public bool IsPublic { get; set; }
        public bool IsFriend { get; set; }
        public bool IsFamily { get; set; }
        public bool HasOriginalPhoto
        {
            get { return !string.IsNullOrEmpty(OriginalSecret) && !string.IsNullOrEmpty(OriginalFormat); }
        }  

        public string ThumbnailUrl
        {
            get
            {
                return GetImageUrl(ImageSize.Small240);
            }
        }

        public string LargeImageUrl
        {
            get { return GetImageUrl(ImageSize.Large1024); }
        }

        public string GetImageUrl(ImageSize imageSize)
        {
            if(imageSize == ImageSize.Original && !HasOriginalPhoto)
                throw new InvalidOperationException("Original version is not available for this photo.");

            return string.Format("http://farm{0}.staticflickr.com/{1}/{2}_{3}_{4}.jpg",
                FarmId, ServerId, Id, imageSize == ImageSize.Original ? OriginalSecret : Secret, GetSizeIdentifier(imageSize));
        }

        private string GetSizeIdentifier(ImageSize imageSize)
        {
            switch (imageSize)
            {
                case ImageSize.Thumbnail100:
                    return "t";
                case ImageSize.Small240:
                    return "m";
                case ImageSize.Small320:
                    return "n";
                case ImageSize.Large1024:
                    return "b";
                case ImageSize.Original:
                    return "o";
            }

            return string.Empty;
        }

    }
}