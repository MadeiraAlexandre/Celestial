using Celestial.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Celestial.Services
{
    public class ApodClient 
    {
        private readonly HttpClient _client;
        private List<APOD> _readList;
        private StorageFile file;
        public ApodClient()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.nasa.gov/planetary/apod")
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        private async void GetApodAsync(DateTime dateTime, APOD data)
        {
            try
            {
                var response = _client.GetAsync($"?date={$"{dateTime.Year.ToString(CultureInfo.InvariantCulture)}-{dateTime.Month.ToString("00", CultureInfo.InvariantCulture)}-{dateTime.Day.ToString("00", CultureInfo.InvariantCulture)}"}&api_key={Credential.CredentialKey}").Result;
                var content = JObject.Parse(await response.Content.ReadAsStringAsync());
                data.Copyright = (string)content["copyright"];
                if (string.IsNullOrEmpty(data.Copyright))
                {
                    data.Copyright = "NASA";
                }
                data.Explanation = (string)content["explanation"];
                data.MediaType = (string)content["media_type"];
                if (data.MediaType != "video")
                {
                    data.HDUrl = (Uri)content["hdurl"];
                }
                data.Title = (string)content["title"];
                data.Url = (Uri)content["url"];
            }
            catch (HttpRequestException ex)
            {
                _ = await new ContentDialog
                {
                    Title = "No Connection",
                    Content = $"{ex.Message}",
                    CloseButtonText = "Close"
                }.ShowAsync();
            }
            catch (AggregateException ex)
            {
                _ = await new ContentDialog
                {
                    Title = "Houston, We Have a Problem",
                    Content = $"{ex.Message}",
                    CloseButtonText = "Close"
                }.ShowAsync();
            }

        }

        public void UpdateFeed(int numberOfUpdates)
        {
            var day = DateTime.UtcNow;
            var daysToRemove = 0;
            var apodList = new List<APOD>(numberOfUpdates);
            var apodArray = InitializeArray<APOD>(numberOfUpdates);
            foreach (var apod in apodArray)
            {
                GetApodAsync(day, apod);
                apod.Date = new DateTime(day.Year, day.Month, day.Day);
                Settings.Instance.NumberOfObjects++;
                if (apod.MediaType == "image")
                {
                    apodList.Add(apod);
                }
                daysToRemove++;
                day = DateTime.UtcNow.AddDays(-daysToRemove);
            }
            WriteData(apodList);
            _client.Dispose();
        }

        private async void WriteData(List<APOD> apodList)
        {
            if (!Settings.Instance.IsFirstLoad)
            {
                apodList.AddRange(await ReadData().ConfigureAwait(false));
            }
            else
            {
                file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Cache.json", CreationCollisionOption.OpenIfExists);
                Settings.Instance.IsFirstLoad = false;
            }

            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            using (StreamWriter sw = new StreamWriter(file.Path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, apodList);
            }

        }

        public async Task<List<APOD>> ReadData()
        {
            file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Cache.json", CreationCollisionOption.OpenIfExists);
            _readList = new List<APOD>(Settings.Instance.NumberOfObjects);
            using (StreamReader reader = new StreamReader(file.Path))
            {
                var jsonString = reader.ReadToEnd();
                _readList = JsonConvert.DeserializeObject<List<APOD>>(jsonString);
            }
            return _readList;
        }

        private static T[] InitializeArray<T>(int lenght) where T : new()
        {
            T[] array = new T[lenght];
            for (int i = 0; i < lenght; i++)
            {
                array[i] = new T();
            }
            return array;
        }

    }
}
