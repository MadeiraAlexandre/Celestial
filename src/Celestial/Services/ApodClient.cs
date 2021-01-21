using Celestial.Models;
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
    public class ApodClient
    {
        //TODO: Use your own key or stay with the demo one.
        private const string api_key = "DEMO_KEY";

        private HttpClient client;

        private bool? isClientRuning;

        private void SetUpClient()
        {
            client = new HttpClient { BaseAddress = new Uri("https://api.nasa.gov/planetary/apod") };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            isClientRuning = true;
        }

        public async Task<List<Apod>> FetchApodListAsync(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (isClientRuning != true) SetUpClient();
            using var response = client.GetAsync($"?api_key={api_key}&start_date={$"{startDate.Year.ToString(CultureInfo.InvariantCulture)}-{startDate.Month.ToString("00", CultureInfo.InvariantCulture)}-{startDate.Day.ToString("00", CultureInfo.InvariantCulture)}"}&end_date={$"{endDate.Year.ToString(CultureInfo.InvariantCulture)}-{endDate.Month.ToString("00", CultureInfo.InvariantCulture)}-{endDate.Day.ToString("00", CultureInfo.InvariantCulture)}"}").Result;
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
    }
}
