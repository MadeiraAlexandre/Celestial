using Celestial.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Celestial.Services
{
    public static class ApodClient
    {

        public static async Task<List<Apod>> FetchApodListAsync(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            try
            {
                var _client = new HttpClient
                {
                    BaseAddress = new Uri("https://api.nasa.gov/planetary/apod")
                };
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var _startDate = $"{startDate.Year.ToString(CultureInfo.InvariantCulture)}-{startDate.Month.ToString("00", CultureInfo.InvariantCulture)}-{startDate.Day.ToString("00", CultureInfo.InvariantCulture)}";
                var _endDate = $"{endDate.Year.ToString(CultureInfo.InvariantCulture)}-{endDate.Month.ToString("00", CultureInfo.InvariantCulture)}-{endDate.Day.ToString("00", CultureInfo.InvariantCulture)}";
                var response = _client.GetAsync(new Uri($"?api_key={Credential.CredentialKey}&start_date={_startDate}&end_date={_endDate}")).Result;
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var content = JArray.Parse(responseContent);
                var results = JsonConvert.DeserializeObject<List<Apod>>(content.ToString());
                results.RemoveAll(x => x.MediaType == "video");
                _client.Dispose();
                return results;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static async Task<Apod> FetchApodAsync(DateTimeOffset date)
        {
            var _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.nasa.gov/planetary/apod")
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = _client.GetAsync(new Uri($"?date={$"{date.Year.ToString(CultureInfo.InvariantCulture)}-{date.Month.ToString("00", CultureInfo.InvariantCulture)}-{date.Day.ToString("00", CultureInfo.InvariantCulture)}"}&api_key={Credential.CredentialKey}")).Result;
            var content = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            var result = JsonConvert.DeserializeObject<Apod>(content.ToString());
            _client.Dispose();
            return result;
        }

    }
}
