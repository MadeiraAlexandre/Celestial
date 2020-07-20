using Celestial.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Celestial
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ContentFrame_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Instance.IsFirstLoad)
            {
                ContentFrame.Navigate(typeof(Views.WelcomeView));
            }
            else
            {
                ContentFrame.Navigate(typeof(Views.GalleryView));

            }
        }
    }
}
