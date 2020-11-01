﻿using Celestial.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

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
            catch (Exception)
            {
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
                }
                return dataList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}