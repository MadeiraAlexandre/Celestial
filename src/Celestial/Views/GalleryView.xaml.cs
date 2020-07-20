using Celestial.Helper;
using Celestial.Model;
using Celestial.Services;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace Celestial.Views
{
    public sealed partial class GalleryView : Page
    {
        private readonly Compositor _compositor = Window.Current.Compositor;
        private SpringVector3NaturalMotionAnimation _springAnimation;
        private APOD _selectedItem;
        private  List<APOD> _apodList;
        private  ApodClient _client;

        public GalleryView()
        {
            InitializeComponent();
            _client = new ApodClient();
            _apodList = new List<APOD>(Model.Settings.Instance.NumberOfObjects);
        }

        private void CreateOrUpdateSpringAnimation(float finalValue)
        {
            if (_springAnimation == null)
            {
                _springAnimation = _compositor.CreateSpringVector3Animation();
                _springAnimation.Target = "Scale";
            }
            _springAnimation.FinalValue = new Vector3(finalValue);
        }

        private void GalleryGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (GalleryGridView.ContainerFromItem(e.ClickedItem) is GridViewItem clickedItem)
            {
                FindName("FullScreenGrid");
                _selectedItem = clickedItem.Content as APOD;
                TitleTextBlock.Text = _selectedItem.Title;
                CopyrightTextBlock.Text = $"by {_selectedItem.Copyright}";
                ExplanationTextBlock.Text = _selectedItem.Explanation;
                Picture.Source = new BitmapImage(_selectedItem.Url);
                FullScreenGrid.Visibility = Visibility.Visible;
                var forwardAnimation = GalleryGridView.PrepareConnectedAnimation("ForwardAnimation", _selectedItem, "GalleryGridViewImage");
                forwardAnimation.Configuration = new DirectConnectedAnimationConfiguration();
                forwardAnimation.TryStart(Picture);
                forwardAnimation.Completed += ForwardAnimation_Completed;
            }
        }

        private void ForwardAnimation_Completed(ConnectedAnimation sender, object args) => InfoPanel.Opacity = 1;

        private void Image_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CreateOrUpdateSpringAnimation(1.06f);
            (sender as UIElement).StartAnimation(_springAnimation);
        }

        private void Image_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CreateOrUpdateSpringAnimation(1.0f);
            (sender as UIElement).StartAnimation(_springAnimation);
        }

        private async void FullScreenGridBackButton_Click(object sender, RoutedEventArgs e)
        {
            GalleryGridView.ScrollIntoView(_selectedItem, ScrollIntoViewAlignment.Default);
            GalleryGridView.UpdateLayout();
            InfoPanel.Opacity = 0;
            var backwardsAnimation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackwardsAnimation", Picture);
            backwardsAnimation.Completed += BackwardsAnimation_Completed;
            backwardsAnimation.Configuration = new DirectConnectedAnimationConfiguration();
            await GalleryGridView.TryStartConnectedAnimationAsync(backwardsAnimation, _selectedItem, "GalleryGridViewImage");
        }

        private void BackwardsAnimation_Completed(ConnectedAnimation sender, object args)
        {
            FullScreenGrid.Visibility = Visibility.Collapsed;
            UnloadObject(FullScreenGrid);
        }

        private async void GalleryGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Model.Settings.Instance.LastFeedUpdate.Day != DateTimeOffset.UtcNow.Day && Model.Settings.Instance.LastFeedUpdate.Month != DateTimeOffset.UtcNow.Month)
            {
                var today = DateTimeOffset.UtcNow;
                var numberOfUpdates = today.Subtract(Model.Settings.Instance.LastFeedUpdate);
                _client.UpdateFeed(numberOfUpdates.Days);
            }
            _apodList = await _client.ReadData().ConfigureAwait(true);
            GalleryGridView.ItemsSource = _apodList;
        }

        private void GalleryGridViewSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            FindName("SettingsGrid");
            SettingsGrid.Visibility = Visibility.Visible;
        }

        private async void PictureFlyoutDownloadItem_Click(object sender, RoutedEventArgs e) => await DownloadHelper.DownloadImage(_selectedItem.HDUrl, _selectedItem.Title).ConfigureAwait(false);

        private async void GalleryGridViewFlyoutDownload_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as APOD;
            await DownloadHelper.DownloadImage(item.HDUrl, item.Title).ConfigureAwait(false);
        }

        private async void PictureFlyoutWallpaperItem_Click(object sender, RoutedEventArgs e) => await PersonalizationHelper.SetAsWallpaper(_selectedItem.HDUrl).ConfigureAwait(false);

        private async void PictureFlyoutLockScreenItem_Click(object sender, RoutedEventArgs e) => await PersonalizationHelper.SetAsLockScreen(_selectedItem.HDUrl).ConfigureAwait(false);

        private void PictureFlyoutShareItem_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = _selectedItem.Title;
            request.Data.Properties.Description = $"Image of {_selectedItem.Title}";
            request.Data.SetWebLink(_selectedItem.Url);
        }

        private void SettingsGridBackButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsGrid.Visibility = Visibility.Collapsed;
            UnloadObject(SettingsGrid);
        }
    }
}
