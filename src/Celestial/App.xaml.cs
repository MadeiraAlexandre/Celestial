using Celestial.Helper;
using Celestial.Model;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Celestial
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                CoreApplication.EnablePrelaunch(true);
                if (rootFrame.Content == null)
                {
                    if (!AppSettings.Instance.IsFirstLoad)
                    {
                        rootFrame.Navigate(typeof(Views.GalleryView), e.Arguments);
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(Views.WelcomeView), e.Arguments);
                    }
                }
                Window.Current.Activate();
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            }
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
