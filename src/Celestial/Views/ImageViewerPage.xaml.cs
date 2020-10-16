using Celestial.Helper;
using Celestial.Model;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Celestial.Views
{
    public sealed partial class ImageViewerPage : Page
    {
        private Apod _selectedItem;
        private bool _isAnimationEnabled;

        public ImageViewerPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _selectedItem = (Apod)e.Parameter;
            var imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("forwardAnimation");
            TitleTextBlock.Text = _selectedItem.Title;
            if (string.IsNullOrEmpty(_selectedItem.Copyright))
            {
                _selectedItem.Copyright = "NASA";
            }
            CopyrightTextBlock.Text = $"by {_selectedItem.Copyright}";
            ExplanationTextBlock.Text = _selectedItem.Explanation;
            Picture.Source = new BitmapImage(_selectedItem.Url);
            Picture.Name = $"Image of {_selectedItem.Title}";
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(Picture);
                imageAnimation.Completed += ImageAnimation_Completed;
                _isAnimationEnabled = true;
            }
            else
            {
                ActionPanel.Opacity = 1;
                DetailsColumn.Opacity = 1;
                _isAnimationEnabled = false;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if(_isAnimationEnabled == true)
            {
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("backwardsAnimation", Picture);
            }
        }

        private void ImageAnimation_Completed(ConnectedAnimation sender, object args)
        {
            ActionPanel.Opacity = 1;
            DetailsColumn.Opacity = 1;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ActionPanel.Opacity = 0;
            DetailsColumn.Opacity = 0;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void DownloadImageUI_Click(object sender, RoutedEventArgs e)
        {
            ActionPanel_DownloadImage.IsEnabled = false;
            ImageViewer_Flyout_Download.IsEnabled = false;
            if (AppSettings.Instance.IsFirstDownload)
            {
                _ = await new ContentDialog
                {
                    Title = "Downloading",
                    Content = "All downloads are saved on your local Downloads folder.",
                    CloseButtonText = "OK"
                }.ShowAsync();
                AppSettings.Instance.IsFirstDownload = false;
            }
            await DownloadHelper.Download(_selectedItem.HdUrl, _selectedItem.Title).ConfigureAwait(true);
        }

        private async void SetWallpaperUI_Click(object sender, RoutedEventArgs e)
        {
            ActionPanel_SetWallpaper.IsEnabled = false;
            ImageViewer_Flyout_SetWallpaper.IsEnabled = false;
            if (AppSettings.Instance.IsFirstWallpaper)
            {
                _ = await new ContentDialog
                {
                    Title = "Defining image as wallpaper",
                    Content = "This may take some seconds depending on your connection, a higher definition picture is being downloaded.",
                    CloseButtonText = "OK"
                }.ShowAsync();
                AppSettings.Instance.IsFirstWallpaper = false;
            }
            await PersonalizationHelper.SetAsWallpaperAsync(_selectedItem.HdUrl).ConfigureAwait(true);
        }

        private void ShareUI_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = _selectedItem.Title;
            request.Data.Properties.Description = $"Link to {_selectedItem.Title}";
            request.Data.SetWebLink(_selectedItem.HdUrl);
        }
    }
}
