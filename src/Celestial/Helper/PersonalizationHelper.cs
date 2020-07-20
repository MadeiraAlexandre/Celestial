using System;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.System.UserProfile;

namespace Celestial.Helper
{
    public static class PersonalizationHelper
    {
        private static StorageFile ImageFile { get; set; }

        public static bool IsPersonalizationSupported => UserProfilePersonalizationSettings.IsSupported();

        private static async Task DownloadTempImage(Uri imageUrl, string fileName)
        {
            ImageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var downloader = new BackgroundDownloader();
            var download = downloader.CreateDownload(imageUrl, ImageFile);
            _ = await download.StartAsync();
        }

        public static async Task SetAsLockScreen(Uri imageUrl)
        {
             await DownloadTempImage(imageUrl, "lockscreen.jpg").ConfigureAwait(false);
             await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(ImageFile);
        }

        public static async Task SetAsWallpaper(Uri imageUrl)
        {
            await DownloadTempImage(imageUrl, "wallpaper.jpg").ConfigureAwait(false);
            await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(ImageFile);
        }
    }
}
