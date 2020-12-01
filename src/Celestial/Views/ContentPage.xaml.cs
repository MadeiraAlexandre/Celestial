using Celestial.Models;
using Celestial.Utils;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Celestial.Views
{
    public sealed partial class ContentPage : Page
    {
        private Apod SelectedItem { get; set; }

        public ContentPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SelectedItem = (Apod)e.Parameter;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }

        private void ShowPanel_Click(object sender, RoutedEventArgs e)
        {
            switch (ImageViewer_Flyout_ShowPanel.IsChecked)
            {
                case true:
                    InfoPanel.Visibility = Visibility.Visible;
                    break;
                case false:
                    InfoPanel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private async void DownloadImage_Click(object sender, RoutedEventArgs e)
        {
            ActionPanel_DownloadImage.IsEnabled = false;
            ImageViewer_Flyout_Download.IsEnabled = false;
            if (Settings.Instance.ShowDownloadNotification)
            {
                ContentPage_Notification.Show("All downloads are saved on your local Downloads folder.", 3500);
                Settings.Instance.ShowDownloadNotification = false;
            }
            await Download.DownloadImageAsync(SelectedItem.HdUrl, SelectedItem.Title).ConfigureAwait(true);
        }

        private async void SetWallpaper_Click(object sender, RoutedEventArgs e)
        {
            ActionPanel_SetWallpaper.IsEnabled = false;
            ImageViewer_Flyout_SetWallpaper.IsEnabled = false;
            if (Settings.Instance.ShowWallpaperNotification)
            {
                ContentPage_Notification.Show("Defining image as wallpaper.\nThis may take some seconds depending on your connection, a higher definition picture is being downloaded.", 3500);
                Settings.Instance.ShowWallpaperNotification = false;
            }
            await Personalization.SetAsWallpaperAsync(SelectedItem.HdUrl).ConfigureAwait(true);
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = SelectedItem.Title;
            request.Data.Properties.Description = $"Link to {SelectedItem.Title}";
            request.Data.SetWebLink(SelectedItem.HdUrl);
        }
    }
}