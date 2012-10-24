using System;

namespace FlickrBrowser.Infrastructure
{
    public class MenuItemClickedEventArgs : EventArgs
    {
       public MenuItemClickedEventArgs(TargetPage targetPage)
        {
            TargetPage = targetPage;
        }

        public TargetPage TargetPage { get; private set; }
    }
}