using Celestial.Shared.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Celestial.Shared.Services
{
    public static class ApodClient
    {
        public static async Task<List<Apod>> FetchApodListAsync(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            using var _client = new HttpClient { BaseAddress = new Uri("https://api.nasa.gov/planetary/apod") };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var _startDate = $"{startDate.Year.ToString(CultureInfo.InvariantCulture)}-{startDate.Month.ToString("00", CultureInfo.InvariantCulture)}-{startDate.Day.ToString("00", CultureInfo.InvariantCulture)}";
            var _endDate = $"{endDate.Year.ToString(CultureInfo.InvariantCulture)}-{endDate.Month.ToString("00", CultureInfo.InvariantCulture)}-{endDate.Day.ToString("00", CultureInfo.InvariantCulture)}";
            using var response = _client.GetAsync($"?api_key={Credential.CredentialKey}&start_date={_startDate}&end_date={_endDate}").Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var content = JArray.Parse(responseContent);
                var results = JsonConvert.DeserializeObject<List<Apod>>(content.ToString());
                results.RemoveAll(x => x.MediaType == "video");
                results.ForEach(delegate (Apod apod) { if (string.IsNullOrEmpty(apod.Copyright)) apod.Copyright = "NASA"; });
                return results;
            }
            else return null;
        }

        public static async Task<Apod> FetchApodAsync(DateTimeOffset date)
        {
            using var _client = new HttpClient { BaseAddress = new Uri("https://api.nasa.gov/planetary/apod") };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using var response = _client.GetAsync($"?date={$"{date.Year.ToString(CultureInfo.InvariantCulture)}-{date.Month.ToString("00", CultureInfo.InvariantCulture)}-{date.Day.ToString("00", CultureInfo.InvariantCulture)}"}&api_key={Credential.CredentialKey}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                var result = JsonConvert.DeserializeObject<Apod>(content.ToString());
                if (string.IsNullOrEmpty(result.Copyright)) result.Copyright = "NASA";
                return result.MediaType == "image" ? result : null;
            }
            else return null;
        }
    }
}