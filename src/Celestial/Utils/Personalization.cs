using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Celestial.Utils
{
    public static class Personalization
    {
        public static async Task SetAsWallpaperAsync(Uri imageUrl)
        {
            var tempFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("wallpaper.jpg", CreationCollisionOption.ReplaceExisting);
            await new Windows.Networking.BackgroundTransfer.BackgroundDownloader().CreateDownload(imageUrl, tempFile).StartAsync();
            _ = await Windows.System.UserProfile.UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(tempFile);
        }
    }
}