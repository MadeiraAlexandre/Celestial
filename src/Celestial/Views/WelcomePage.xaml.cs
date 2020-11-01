using Celestial.Models;
using Celestial.Services;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Celestial.Views
{
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage() => InitializeComponent();

        private async void WelcomeGridCloseButton_Click(object sender, RoutedEventArgs e)
        {
            WelcomeGridCloseButton.IsEnabled = false;
            WelcomeGridCloseButton.Content = "Loading images";
            await Task.Delay(100).ConfigureAwait(true);
            var dataList = await ApodClient.FetchApodListAsync(DateTimeOffset.Now.AddMonths(-1), DateTimeOffset.UtcNow).ConfigureAwait(true);
            await CacheData.WriteCacheAsync(dataList).ConfigureAwait(true);
            AppSettings.Instance.IsFirstLoad = false;
            Frame.Navigate(typeof(MainPage), new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void AnimatedVisualPlayer_Loaded(object sender, RoutedEventArgs e) => WelcomeGridCloseButton.IsEnabled = true;
    }
}