using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FlickrBrowser.Common;
using FlickrBrowser.Infrastructure;

namespace FlickrBrowser.DataModel
{
    public class FlickrHelpers
    {
        public static IEnumerable<FlickrPhoto> CreateFlickrPhotoFromXml(XElement xml)
        {
            return from element in xml.Descendants("photo")
                   select new FlickrPhoto
                   {
                       Id = element.Attribute("id").AttributeValueOrDefault(),
                       Owner = element.Attribute("owner").AttributeValueOrDefault(),
                       OwnerName = element.Attribute("ownername").AttributeValueOrDefault(),
                       DateCreated = GetDateTimeFromUnixTimestamp(element.Attribute("dateupload").AttributeValueOrDefault()),
                       DateTaken = element.Attribute("datetaken").AttributeValueOrDefault("Unknown"),
                       OriginalFormat = element.Attribute("originalformat").AttributeValueOrDefault(),
                       Secret = element.Attribute("secret").AttributeValueOrDefault(),
                       OriginalSecret = element.Attribute("originalsecret").AttributeValueOrDefault(),
                       ServerId = element.Attribute("server").AttributeValueOrDefault(),
                       FarmId = element.Attribute("farm").AttributeValueOrDefault(),
                       Title = element.Attribute("title").AttributeValueOrDefault().DefaultIfNullOrEmpty("<no title>"),
                       IsPublic = element.Attribute("ispublic").AttributeValueOrDefault() == "1",
                       IsFamily = element.Attribute("isfamily").AttributeValueOrDefault() == "1",
                       IsFriend = element.Attribute("isfriend").AttributeValueOrDefault() == "1"
                   };
        }

        private static DateTime GetDateTimeFromUnixTimestamp(string unixTimestamp)
        {
            double timestamp;
            if(!double.TryParse(unixTimestamp, out timestamp))
                return DateTime.MinValue;

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(timestamp).ToLocalTime();
        }

     
    }
}
