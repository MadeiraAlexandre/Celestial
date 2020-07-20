using Celestial.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Celestial.Views
{
    public sealed partial class WelcomeView : Page
    {
        private ApodClient _client;
        public WelcomeView()
        {
            this.InitializeComponent();
            _client = new ApodClient();
        }

        private void WelcomeGridCloseButton_Click(object sender, RoutedEventArgs e)
        {
            WelcomeGridCloseButton.Content = "Loading images";
            WelcomeGridCloseButton.IsEnabled = false;
            Frame.Navigate(typeof(GalleryView));
        }

        private void WelcomeGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void AnimatedVisualPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            _client.UpdateFeed(20);
        }
    }
}
