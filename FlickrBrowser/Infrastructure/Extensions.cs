using System;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace FlickrBrowser.Infrastructure
{
    public static class Extensions
    {
        public static string AttributeValueOrDefault(this XAttribute attribute, string defaultValue = "")
        {
            if(attribute == null)
                return defaultValue;

            return attribute.Value;
        }


        public static string DefaultIfNullOrEmpty(this string str, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            return str;
        }

        public static FrameworkElement FindDescendantByName(this FrameworkElement element, string name)
        {
            if (element == null || string.IsNullOrWhiteSpace(name)) { return null; }

            if (name.Equals(element.Name, StringComparison.OrdinalIgnoreCase))
            {
                return element;
            }
            var childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                var result = (VisualTreeHelper.GetChild(element, i) as FrameworkElement).FindDescendantByName(name);
                if (result != null) { return result; }
            }
            return null;
        }
    }
}
