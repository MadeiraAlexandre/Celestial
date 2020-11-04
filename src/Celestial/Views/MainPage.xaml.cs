using Celestial.Models;
using Celestial.Services;
using Celestial.Shared.Models;
using Celestial.Shared.Services;
using Microsoft.Toolkit.Uwp;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Celestial.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private readonly Compositor _compositor = Window.Current.Compositor;
        private SpringVector3NaturalMotionAnimation _springAnimation;
        private readonly IncrementalLoadingCollection<ApodSource, Apod> collection;
        private Apod _selectedItem;
        private Apod SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged();
            }
        }
        private DateTimeOffset MaxDateAllowed => DateTimeOffset.UtcNow;
        private bool _isSearch;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            if (Settings.Instance.ShowWelcomeGrid) FindName("WelcomeGrid");
            if (collection == null) collection = new IncrementalLoadingCollection<ApodSource, Apod>();
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

        private void NotifyPropertyChanged([CallerMemberName] string propName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        private void ImageGrid_Image_Loaded(object sender, RoutedEventArgs e)
        {
            ImageGrid_Image.Name = $"Image of {SelectedItem.Title}";
            ImageGrid_InfoPanel.Opacity = 1;
            ImageGrid_BackButton.Opacity = 1;
        }

        private void WelcomeGrid_Loaded(object sender, RoutedEventArgs e) => WelcomePage_Description.Text = "Explore the universe with high-quality pictures of the cosmos, alongside with an explanation by a professional.\nThis application is unofficial and not affiliated with NASA in any way. Some images and their explanation might be protected by copyright.";

        private void GalleryGrid_GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (GalleryGrid_GridView.ContainerFromItem(e.ClickedItem) is GridViewItem clickedItem)
            {
                SelectedItem = clickedItem.Content as Apod;
                _isSearch = false;
                FindName("ImageGrid");
                var connectedAnimation = GalleryGrid_GridView.PrepareConnectedAnimation("forwardAnimation", SelectedItem, "GalleryGridView_GridView_Image");
                connectedAnimation.Configuration = new DirectConnectedAnimationConfiguration();
                connectedAnimation.TryStart(ImageGrid_Image);
            }
        }

        private async void CDP_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            SelectedItem = await ApodClient.FetchApodAsync((DateTimeOffset)sender.Date).ConfigureAwait(true);
            if (SelectedItem != null)
            {
                FindName("ImageGrid");
                _isSearch = true;
            }
            else _ = await new ContentDialog { Title = "Houston, we have a problem", Content = "Please, try again later", CloseButtonText = "OK" }.ShowAsync();
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
                    GalleryGrid_GridView.ScrollIntoView(SelectedItem, ScrollIntoViewAlignment.Default);
                    GalleryGrid_GridView.UpdateLayout();
                    var connectedAnimation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("backwardsAnimation", ImageGrid_Image);
                    connectedAnimation.Configuration = new DirectConnectedAnimationConfiguration();
                    await GalleryGrid_GridView.TryStartConnectedAnimationAsync(connectedAnimation, SelectedItem, "GalleryGridView_GridView_Image");
                    break;
            }
            UnloadObject(ImageGrid);
        }

        private void ImageViewer_Flyout_ShowPanel_Click(object sender, RoutedEventArgs e)
        {
            switch (ImageViewer_Flyout_ShowPanel.IsChecked)
            {
                case true:
                    ImageGrid_InfoPanel.Visibility = Visibility.Visible;
                    break;
                case false:
                    ImageGrid_InfoPanel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private async void WelcomeGridCloseButton_Click(object sender, RoutedEventArgs e)
        {
            WelcomeGridCloseButton.IsEnabled = false;
            WelcomeGridCloseButton.Content = "Loading images";
            Settings.Instance.ShowWelcomeGrid = false;
            await System.Threading.Tasks.Task.Delay(50).ConfigureAwait(true);
            UnloadObject(WelcomeGrid);
        }

        private async void DownloadImageUI_Click(object sender, RoutedEventArgs e)
        {
            ActionPanel_DownloadImage.IsEnabled = false;
            ImageViewer_Flyout_Download.IsEnabled = false;
            if (Settings.Instance.ShowDownloadNotification)
            {
                ImageGrid_Notification.Show("All downloads are saved on your local Downloads folder.", 3000);
                Settings.Instance.ShowDownloadNotification = false;
            }
            await Download.DownloadImageAsync(SelectedItem.HdUrl, SelectedItem.Title).ConfigureAwait(true);
        }

        private async void SetWallpaperUI_Click(object sender, RoutedEventArgs e)
        {
            ActionPanel_SetWallpaper.IsEnabled = false;
            ImageViewer_Flyout_SetWallpaper.IsEnabled = false;
            if (Settings.Instance.ShowWallpaperNotification)
            {
                ImageGrid_Notification.Show("Defining image as wallpaper.\nThis may take some seconds depending on your connection, a higher definition picture is being downloaded.", 3000);
                Settings.Instance.ShowWallpaperNotification = false;
            }
            await Personalization.SetAsWallpaperAsync(SelectedItem.HdUrl).ConfigureAwait(true);
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
            request.Data.Properties.Title = SelectedItem.Title;
            request.Data.Properties.Description = $"Link to {SelectedItem.Title}";
            request.Data.SetWebLink(SelectedItem.HdUrl);
        }

        private async void GalleryGridViewFlyout_Download_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as Apod;
            await Download.DownloadImageAsync(item.HdUrl, item.Title).ConfigureAwait(true);
        }
    }
}