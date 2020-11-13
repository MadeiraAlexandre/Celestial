using Newtonsoft.Json;
using System;

namespace Celestial.Shared.Models
{
    public class Apod
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "explanation")]
        public string Explanation { get; set; }

        [JsonProperty(PropertyName = "copyright")]
        public string Copyright { get; set; }

        [JsonProperty(PropertyName = "media_type")]
        public string MediaType { get; set; }

        [JsonProperty(PropertyName = "url")]
        public Uri Url { get; set; }

        [JsonProperty(PropertyName = "hdurl")]
        public Uri HdUrl { get; set; }

        [JsonProperty(PropertyName = "date")]
        private string DateString { get; set; }

        public DateTime Date => DateTime.Parse(DateString, new System.Globalization.CultureInfo("en-US"));
    }
}