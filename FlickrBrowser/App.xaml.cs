using FlickrBrowser.Common;
using System;
using FlickrBrowser.DataModel;
using FlickrBrowser.Infrastructure;
using FlickrBrowser.Pages;
using FlickrBrowser.UserControls;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Search;

// The Split App template is documented at http://go.microsoft.com/fwlink/?LinkId=234228

namespace FlickrBrowser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            FlickrApi.Instance.ApiErrorOccured += ErrorHandler;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                if (!rootFrame.Navigate(typeof(HomePage), "AllGroups"))
                {
                    throw new Exception("Failed to create initial page");
                }
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!string.IsNullOrEmpty(RoamingAppSettings.Instance.HighPriority))
                {
                    if (RoamingAppSettings.Instance.HighPriority.Equals(TargetPage.Home.ToString()))
                    {
                        if (!rootFrame.Navigate(typeof(HomePage), "AllGroups"))
                        {
                            throw new Exception("Failed to create initial page");
                        }
                    }
                    else if (RoamingAppSettings.Instance.HighPriority.Equals(TargetPage.RecentlyAdded.ToString()))
                    {
                        if (!rootFrame.Navigate(typeof(RecentlyAddedPage), "AllGroups"))
                        {
                            throw new Exception("Failed to create initial page");
                        }
                    }
                    else if (RoamingAppSettings.Instance.HighPriority.Equals(TargetPage.MyPhotos.ToString()))
                    {
                        if (!rootFrame.Navigate(typeof(MyPhotosPage), "AllGroups"))
                        {
                            throw new Exception("Failed to create initial page");
                        }
                    }
                    
                }
            }
            
            // Ensure the current window is active
            Window.Current.Activate();
        }

        private static async void ErrorHandler(object sender, ApiErrorOccuredEventArgs e)
        {
            await new MessageDialog("An error ocurred: " + e.ErrorMessage).ShowAsync();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
            FlickrApi.Instance.ApiErrorOccured -= ErrorHandler;
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);

            BindQuerySubmitted();
        }

        private static void BindQuerySubmitted()
        {
            var searchPane = SearchPane.GetForCurrentView();

            if (searchPane != null)
            {
                searchPane.QuerySubmitted += (o, e) =>
                {
                    var frame = Window.Current.Content as Frame;
                    if (frame != null)
                        frame.Navigate(typeof(SearchResultsPage), e.QueryText);
                };
            }
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // event in OnWindowCreated to speed up searches once the application is already running

            // If the Window isn't already using Frame navigation, insert our own Frame
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            // If the app does not contain a top-level frame, it is possible that this 
            // is the initial launch of the app. Typically this method and OnLaunched 
            // in App.xaml.cs can call a common method.
            if (frame == null)
            {
                // Create a Frame to act as the navigation context and associate it with
                // a SuspensionManager key
                frame = new Frame();
                FlickrBrowser.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await FlickrBrowser.Common.SuspensionManager.RestoreAsync();
                    }
                    catch (FlickrBrowser.Common.SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }

            if (string.IsNullOrEmpty(args.QueryText))
                frame.Navigate(typeof(HomePage));
            else
                frame.Navigate(typeof(SearchResultsPage), args.QueryText);

            Window.Current.Content = frame;
        }
            
        protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(PhotoRecievedPage), args.ShareOperation);
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }
    }
}
