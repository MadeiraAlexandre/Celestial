using Newtonsoft.Json;
using System;
using Windows.UI.Xaml.Media.Imaging;

namespace Celestial.Models
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
        private Uri Url { get; set; }

        public BitmapImage Image => new BitmapImage(Url);

        [JsonProperty(PropertyName = "hdurl")]
        public Uri HdUrl { get; set; }

        [JsonProperty(PropertyName = "date")]
        private string DateString { get; set; }

        public DateTime Date => DateTime.Parse(DateString, new System.Globalization.CultureInfo("en-US"));
    }
}
