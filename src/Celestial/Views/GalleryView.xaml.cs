using Celestial.Helper;
using Celestial.Model;
using Celestial.Services;
using Microsoft.Toolkit.Uwp;
using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Celestial.Views
{
    public sealed partial class GalleryView : Page
    {
        private readonly Compositor _compositor = Window.Current.Compositor;
        private SpringVector3NaturalMotionAnimation _springAnimation;
        private Apod _selectedItem;
        private Apod _searchItem;
        private IncrementalLoadingCollection<ApodSource, Apod> collection;

        public GalleryView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
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

        private void GalleryGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (GalleryGridView.ContainerFromItem(e.ClickedItem) is GridViewItem clickedItem)
            {
                _selectedItem = clickedItem.Content as Apod;
                var animation = GalleryGridView.PrepareConnectedAnimation("forwardAnimation", _selectedItem, "GalleryGridViewImage");
                Frame.Navigate(typeof(ImageViewerPage), _selectedItem);
            }
        }

        private async void GalleryGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_selectedItem != null)
            {
                GalleryGridView.ScrollIntoView(_selectedItem, ScrollIntoViewAlignment.Default);
                GalleryGridView.UpdateLayout();

                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("backwardsAnimation");
                if (animation != null)
                {
                    await GalleryGridView.TryStartConnectedAnimationAsync(animation, _selectedItem, "GalleryGridViewImage");
                }
            }
            if (collection == null)
            {
                collection = new IncrementalLoadingCollection<ApodSource, Apod>();
                GalleryGridView.ItemsSource = collection;
            }
        }

        private void GalleryGridViewSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            FindName("SettingsGrid");
            SettingsGrid.Visibility = Visibility.Visible;
        }

        private void SettingsGridBackButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsGrid.Visibility = Visibility.Collapsed;
            UnloadObject(SettingsGrid);
        }

        private void CSPicker_Loaded(object sender, RoutedEventArgs e) => CSPicker.MaxDate = DateTimeOffset.UtcNow;

        private async void GalleryGridViewFlyout_Download_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as Apod;
            await DownloadHelper.Download(item.HdUrl, item.Title).ConfigureAwait(true);
        }

        private async void CSPicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            _searchItem = await ApodClient.FetchApodAsync((DateTimeOffset)sender.Date).ConfigureAwait(true);
            Frame.Navigate(typeof(ImageViewerPage), _searchItem);
        }
    }
}
