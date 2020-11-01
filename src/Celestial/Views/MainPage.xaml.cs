using Celestial.Helpers;
using Celestial.Models;
using Celestial.Services;
using Microsoft.Toolkit.Uwp;
using System;
using System.Linq;
using System.Numerics;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Celestial.Views
{
    public sealed partial class MainPage : Page
    {
        private readonly Compositor _compositor = Window.Current.Compositor;
        private SpringVector3NaturalMotionAnimation _springAnimation;
        private Apod _searchItem;
        private IncrementalLoadingCollection<ApodSource, Apod> collection;
        private Apod _selectedItem;
        private bool _isSearch;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
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

        private void GalleryGrid_GridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (collection == null)
            {
                collection = new IncrementalLoadingCollection<ApodSource, Apod>();
                GalleryGrid_GridView.ItemsSource = collection;
                collection.OrderByDescending(o => o.Date);
            }
            CDP.MaxDate = DateTimeOffset.UtcNow;
            GalleryGrid_GridView_Header.Opacity = 1;
        }

        private void GalleryGrid_GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (GalleryGrid_GridView.ContainerFromItem(e.ClickedItem) is GridViewItem clickedItem)
            {
                _selectedItem = clickedItem.Content as Apod;
                FindName("ImageGrid");
                ImageGrid_Image.Source = new BitmapImage(_selectedItem.Url);
                TitleTextBlock.Text = _selectedItem.Title;
                if (string.IsNullOrEmpty(_selectedItem.Copyright)) _selectedItem.Copyright = "NASA";
                CopyrightTextBlock.Text = $"by {_selectedItem.Copyright}";
                ExplanationTextBlock.Text = _selectedItem.Explanation;
                var connectedAnimation = GalleryGrid_GridView.PrepareConnectedAnimation("forwardAnimation", _selectedItem, "GalleryGridView_GridView_Image");
                connectedAnimation.Configuration = new DirectConnectedAnimationConfiguration();
                connectedAnimation.TryStart(ImageGrid_Image);
                connectedAnimation.Completed += ForwardConnectedAnimation_Completed;
                ImageGrid.Visibility = Visibility.Visible;
                _isSearch = false;
            }
        }

        private void ForwardConnectedAnimation_Completed(ConnectedAnimation sender, object args)
        {
            ImageGrid_InfoPanel.Opacity = 1;
            ImageGrid_BackButton.Opacity = 1;
        }

        private async void ImageGrid_BackButton_Click(object sender, RoutedEventArgs e)
        {
            ImageGrid_InfoPanel.Opacity = 0;
            ImageGrid_BackButton.Opacity = 0;
            switch (_isSearch)
            {
                case true:
                    _isSearch = false;
                    break;
                case false:
                    GalleryGrid_GridView.ScrollIntoView(_selectedItem, ScrollIntoViewAlignment.Default);
                    GalleryGrid_GridView.UpdateLayout();
                    var connectedAnimation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("backwardsAnimation", ImageGrid_Image);
                    connectedAnimation.Configuration = new DirectConnectedAnimationConfiguration();
                    await GalleryGrid_GridView.TryStartConnectedAnimationAsync(connectedAnimation, _selectedItem, "GalleryGridView_GridView_Image");
                    break;
            }
            UnloadObject(ImageGrid);
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

        private void SettingsGrid_BackButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsGrid.Visibility = Visibility.Collapsed;
            UnloadObject(SettingsGrid);
        }

        private void GalleryGrid_AboutButton_Click(object sender, RoutedEventArgs e)
        {
            FindName("SettingsGrid");
            SettingsGrid.Visibility = Visibility.Visible;
        }

        private async void CDP_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            _searchItem = await ApodClient.FetchApodAsync((DateTimeOffset)sender.Date).ConfigureAwait(true);
            if (_searchItem != null || _searchItem.MediaType != "video")
            {
                FindName("ImageGrid");
                ImageGrid_Image.Source = new BitmapImage(_searchItem.Url);
                TitleTextBlock.Text = _searchItem.Title;
                if (string.IsNullOrEmpty(_searchItem.Copyright)) _searchItem.Copyright = "NASA";
                CopyrightTextBlock.Text = $"by {_searchItem.Copyright}";
                ExplanationTextBlock.Text = _searchItem.Explanation;
                ImageGrid.Visibility = Visibility.Visible;
                ImageGrid_InfoPanel.Opacity = 1;
                _isSearch = true;
            }
            else
            {
                _ = await new ContentDialog
                {
                    Title = "Houston, we have a problem",
                    Content = "Please, try again later",
                    CloseButtonText = "OK"
                }.ShowAsync();
            }
        }

        private async void GalleryGridViewFlyout_Download_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as Apod;
            await DownloadHelper.Download(item.HdUrl, item.Title).ConfigureAwait(true);
        }

        private void SettingsGrid_Loaded(object sender, RoutedEventArgs e) => SettingsGrid_AboutDescriptionText.Text = "This application is only possible thanks to NASA Open APIs.\nCelestial is open source and available on GitHub under the MIT License.";

        private void ImageGrid_Image_Loaded(object sender, RoutedEventArgs e) => ImageGrid_Image.Name = $"Image of {_selectedItem.Title}";

        private void ImageViewer_Flyout_ShowPanel_Click(object sender, RoutedEventArgs e)
        {
            switch (ImageViewer_Flyout_ShowPanel.IsChecked)
            {
                case true:
                    ImageGrid_InfoPanel.Opacity = 1;
                    break;
                case false:
                    ImageGrid_InfoPanel.Opacity = 0;
                    break;
            }
        }
    }
}