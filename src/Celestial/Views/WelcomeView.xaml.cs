using Celestial.Model;
using Celestial.Services;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Celestial.Views
{
    public sealed partial class WelcomeView : Page
    {
        public WelcomeView() => InitializeComponent();

        private async void WelcomeGridCloseButton_Click(object sender, RoutedEventArgs e)
        {
            WelcomeGridCloseButton.IsEnabled = false;
            WelcomeGridCloseButton.Content = "Loading images";
            await Task.Delay(100).ConfigureAwait(true);
            var dataList = await ApodClient.FetchApodListAsync(DateTimeOffset.Now.AddMonths(-2), DateTimeOffset.UtcNow).ConfigureAwait(true);
            await CacheData.WriteCacheAsync(dataList).ConfigureAwait(true);
            AppSettings.Instance.IsFirstLoad = false;
            Frame.Navigate(typeof(GalleryView));
        }

        private void AnimatedVisualPlayer_Loaded(object sender, RoutedEventArgs e) => WelcomeGridCloseButton.IsEnabled = true;
    }
}
