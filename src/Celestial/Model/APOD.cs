using System;
using System.Runtime.Serialization;

namespace Celestial.Model
{
    [DataContract]
    public class APOD
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Copyright { get; set; }

        [DataMember]
        public string Explanation { get; set; }

        [DataMember]
        public string MediaType { get; set; }

        [DataMember]
        public Uri Url { get; set; }

        [DataMember]
        public Uri HDUrl { get; set; }

        [DataMember]
        public DateTime Date { get; set; }
    }
}
