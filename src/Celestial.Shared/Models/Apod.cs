using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Celestial.Shared.Models
{
    public class Apod
    {
        [DataMember]
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "explanation")]
        public string Explanation { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "copyright")]
        public string Copyright { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "media_type")]
        public string MediaType { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "url")]
        public Uri Url { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "hdurl")]
        public Uri HdUrl { get; set; }

        [JsonProperty(PropertyName = "date")]
        private string DateString { get; set; }

        [DataMember]
        public DateTime Date => DateTime.Parse(DateString, new System.Globalization.CultureInfo("en-US"));
    }
}