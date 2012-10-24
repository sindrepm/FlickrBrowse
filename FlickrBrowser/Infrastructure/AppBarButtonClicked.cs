using System;

namespace FlickrBrowser.Infrastructure
{
    public class AppBarButtonClicked : EventArgs
    {
        public AppBarCommand AppBarCommand { get; set; }
    }
}