using Celestial.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Celestial.Services
{
    public static class CacheData
    {
        private static StorageFile _file;

        public static async Task WriteCacheAsync(List<Apod> dataList)
        {
            try
            {
                _file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Cache.json", CreationCollisionOption.OpenIfExists);
                if (!AppSettings.Instance.IsFirstLoad && dataList != null)
                {
                    dataList.AddRange(await ReadCacheAsync().ConfigureAwait(false));
                    dataList.OrderByDescending(o => o.Date);
                }
                var apodList = dataList.Distinct(new ItemEqualityComparer());
                JsonSerializer serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };
                using (StreamWriter sw = new StreamWriter(_file.Path))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, apodList);
                }
            }
            catch (Exception ex)
            {
                _ = await new ContentDialog
                {
                    Title = "Houston, we have a problem",
                    Content = $"{ex.Message}",
                    CloseButtonText = "Close"
                }.ShowAsync();
                throw;
            }
        }

        public static async Task<List<Apod>> ReadCacheAsync()
        {
            try
            {
                _file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Cache.json", CreationCollisionOption.OpenIfExists);
                var dataList = new List<Apod>();
                using (StreamReader reader = new StreamReader(_file.Path))
                {
                    dataList = JsonConvert.DeserializeObject<List<Apod>>(reader.ReadToEnd());
                    dataList.OrderByDescending(o => o.Date);
                }
                return dataList;
            }
            catch (Exception ex)
            {
                _ = await new ContentDialog
                {
                    Title = "Houston, we have a problem",
                    Content = $"{ex.Message}",
                    CloseButtonText = "Close"
                }.ShowAsync();
                throw;
            }
        }
    }
}