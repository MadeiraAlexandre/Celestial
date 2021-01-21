using Celestial.Models;
using Celestial.Utils;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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
            if (Settings.Instance.IsOriginalAspectRatio) ContentPage_ImageViewer.Stretch = Stretch.Uniform;
            if (Settings.Instance.SwitchToCompactPanel)
            {
                InfoPanel.Visibility = Visibility.Collapsed;
                ImageViewer_Flyout_ShowPanel.IsChecked = false;
            }
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
                    Settings.Instance.SwitchToCompactPanel = true;
                    break;
            }
        }

        private async void DownloadImage_Click(object sender, RoutedEventArgs e)
        {
            ActionPanel_DownloadImage.IsEnabled = false;
            ImageViewer_Flyout_Download.IsEnabled = false;
            if (Settings.Instance.ShowDownloadInfo)
            {
                ContentPage_InfoBar.Title = "Downloading";
                ContentPage_InfoBar.Message = "All downloads are saved on your Pictures folder.";
                ContentPage_InfoBar.IsOpen = true;
                await Task.Delay(3500).ConfigureAwait(true);
                ContentPage_InfoBar.IsOpen = false;
                Settings.Instance.ShowDownloadInfo = false;
            }
            await Download.DownloadImageAsync(SelectedItem.HdUrl, SelectedItem.Title).ConfigureAwait(true);
        }

        private async void SetWallpaper_Click(object sender, RoutedEventArgs e)
        {
            ActionPanel_SetWallpaper.IsEnabled = false;
            ImageViewer_Flyout_SetWallpaper.IsEnabled = false;
            if (Settings.Instance.ShowWallpaperInfo)
            {
                ContentPage_InfoBar.Title = "Defining image as wallpaper";
                ContentPage_InfoBar.Message = "This may take some seconds depending on your connection, a higher definition picture is being downloaded.";
                ContentPage_InfoBar.IsOpen = true;
                await Task.Delay(3500).ConfigureAwait(true);
                ContentPage_InfoBar.IsOpen = false;
                Settings.Instance.ShowWallpaperInfo = false;
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

        private void Image_ImageOpened(object sender, RoutedEventArgs e) => ContentPage_ProgressRing.IsActive = false;

        private void CompactPanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Settings.Instance.SwitchToCompactPanel = false;
            InfoPanel.Visibility = Visibility.Visible;
        }

        private void InfoPanel_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            Settings.Instance.SwitchToCompactPanel = true;
            InfoPanel.Visibility = Visibility.Collapsed;
        }
    }
}