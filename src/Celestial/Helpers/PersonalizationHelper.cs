using System;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.System.UserProfile;

namespace Celestial.Helpers
{
    public static class PersonalizationHelper
    {
        public static bool IsPersonalizationSupported => UserProfilePersonalizationSettings.IsSupported();

        public static async Task SetAsWallpaperAsync(Uri imageUrl)
        {
            var tempFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("wallpaper.jpg", CreationCollisionOption.ReplaceExisting);
            await new BackgroundDownloader().CreateDownload(imageUrl, tempFile).StartAsync();
            _ = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(tempFile);
        }
    }
}
