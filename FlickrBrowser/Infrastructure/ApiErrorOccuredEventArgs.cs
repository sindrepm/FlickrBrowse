using System;

namespace FlickrBrowser.Infrastructure
{
    public class ApiErrorOccuredEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }

        public ApiErrorOccuredEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}