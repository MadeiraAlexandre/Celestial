using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace Celestial.Services
{
    public static class Download
    {
        private static StorageFile DownloadFile { get; set; }

        public static async Task DownloadImageAsync(Uri downloadUrl, string fileName)
        {
            try { DownloadFile = await DownloadsFolder.CreateFileAsync($"{fileName}.jpg", CreationCollisionOption.GenerateUniqueName); }
            catch (FileNotFoundException) { DownloadFile = await DownloadsFolder.CreateFileAsync($"Image.jpg", CreationCollisionOption.GenerateUniqueName); }
            finally
            {
                var downloadOperation = new BackgroundDownloader().CreateDownload(downloadUrl, DownloadFile);
                await downloadOperation.StartAsync();
                downloadOperation.Priority = BackgroundTransferPriority.High;
            }
        }
    }
}