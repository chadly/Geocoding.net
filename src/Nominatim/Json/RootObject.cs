using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Json
{
    [DataContract]
    public class RootObject
    {
        [DataMember(Name = "place_id")]
        public string PlaceId { get; set; }
        [DataMember(Name = "licence")]
        public string Licence { get; set; }
        [DataMember(Name = "osm_type")]
        public string OsmType { get; set; }
        [DataMember(Name = "osm_id")]
        public string OsmId { get; set; }
        [DataMember(Name = "boundingbox")]
        public List<string> BoundingBox { get; set; }
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }
        [DataMember(Name = "lon")]
        public double Longitude { get; set; }
        [DataMember(Name = "display_name")]
        public string DisplayName { get; set; }
        [DataMember(Name = "class")]
        public string Class { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "importance")]
        public double Importance { get; set; }
        [DataMember(Name = "address")]
        public Address Address { get; set; }
    }
}