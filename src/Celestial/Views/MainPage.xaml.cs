using Celestial.Models;
using Celestial.Utils;
using Microsoft.Toolkit.Uwp;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Celestial.Views
{
    public sealed partial class MainPage : Page
    {
        private readonly Compositor _compositor = Window.Current.Compositor;
        private SpringVector3NaturalMotionAnimation _springAnimation;
        private readonly IncrementalLoadingCollection<ApodIncrementalSource, Apod> collection;
        private Apod SelectedItem { get; set; }

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            if (Settings.Instance.ShowWelcomeGrid) FindName("WelcomeGrid");
            if (collection == null) collection = new IncrementalLoadingCollection<ApodIncrementalSource, Apod>();
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
            CreateOrUpdateSpringAnimation(1.04f);
            (sender as UIElement).StartAnimation(_springAnimation);
        }

        private void Image_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CreateOrUpdateSpringAnimation(1.0f);
            (sender as UIElement).StartAnimation(_springAnimation);
        }

        private void WelcomeGrid_Loaded(object sender, RoutedEventArgs e) => WelcomePage_Description.Text = "Celestial (β) delivers a new high-quality image of our cosmos every day, alongside an explanation by a professional.\nThis application is unofficial and not affiliated with NASA in any way.\nSome images and their explanation might be protected by copyright.";

        private void Apod_GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Apod_GridView.ContainerFromItem(e.ClickedItem) is GridViewItem clickedItem)
            {
                SelectedItem = clickedItem.Content as Apod;
                Frame.Navigate(typeof(ContentPage), SelectedItem, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                Apod_GridView.ScrollIntoView(SelectedItem, ScrollIntoViewAlignment.Default);
            }
        }

        private void WelcomeGridCloseButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Instance.ShowWelcomeGrid = false;
            UnloadObject(WelcomeGrid);
        }
    }
}