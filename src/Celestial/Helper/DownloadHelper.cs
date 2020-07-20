using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Celestial.Helper
{
    public static class DownloadHelper
    {
        private static StorageFile ImageFile { get; set; }

        private static async Task Download(Uri imageUri)
        {
            new BackgroundDownloader().CreateDownload(imageUri, ImageFile).Priority = BackgroundTransferPriority.High;
            _ = await new BackgroundDownloader().CreateDownload(imageUri, ImageFile).StartAsync();
        }

        public static async Task DownloadImage(Uri imageUri, string fileName = "APOD.jpg")
        {
            try
            {
                ImageFile = await DownloadsFolder.CreateFileAsync($"{fileName}.jpg", CreationCollisionOption.GenerateUniqueName);
                await Download(imageUri).ConfigureAwait(false);
            }
            catch (FileNotFoundException)
            {
                ImageFile = await DownloadsFolder.CreateFileAsync($"APOD.jpg", CreationCollisionOption.GenerateUniqueName);
                await Download(imageUri).ConfigureAwait(false);
            }
            catch (AggregateException)
            {
                _ = await new ContentDialog
                {
                    Title = $"Failed to download {fileName}",
                    CloseButtonText = "Ok"
                }.ShowAsync();
            }
        }
    }
}
