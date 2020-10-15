using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace Celestial.Helper
{
    public static class DownloadHelper
    {
        private static StorageFile DownloadFile { get; set; }

        public static async Task Download(Uri downloadUrl, string fileName)
        {
            try
            {
                DownloadFile = await DownloadsFolder.CreateFileAsync($"{fileName}.jpg", CreationCollisionOption.GenerateUniqueName);
            }
            catch (FileNotFoundException)
            {
                DownloadFile = await DownloadsFolder.CreateFileAsync($"APOD.jpg", CreationCollisionOption.GenerateUniqueName);
            }
            finally
            {
                var downloadOperation = new BackgroundDownloader().CreateDownload(downloadUrl, DownloadFile);
                await downloadOperation.StartAsync();
                downloadOperation.Priority = BackgroundTransferPriority.High;
            }
        }
    }
}
