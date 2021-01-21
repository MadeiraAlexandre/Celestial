using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;

namespace Celestial.Utils
{
    public static class Download
    {
        private static StorageFile DownloadFile { get; set; }

        public static async Task DownloadImageAsync(Uri downloadUrl, string fileName)
        {
            try
            {
                var downloadFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("Celestial", CreationCollisionOption.OpenIfExists);
                DownloadFile = await downloadFolder.CreateFileAsync($"{fileName}.jpg", CreationCollisionOption.GenerateUniqueName);
            }
            catch (FileNotFoundException) { DownloadFile = await DownloadsFolder.CreateFileAsync($"Image.jpg", CreationCollisionOption.GenerateUniqueName); }
            catch (UnauthorizedAccessException)
            {
                _ = await new MessageDialog("The app needs to acess picture library to store downloaded images.", "Permission nedded").ShowAsync();
                await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
            }
            finally { await new BackgroundDownloader().CreateDownload(downloadUrl, DownloadFile).StartAsync(); }
        }
    }
}