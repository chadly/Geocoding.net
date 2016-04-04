using System.Runtime.Serialization;

namespace Geocoding.GeoNames.Json
{
    [DataContract]
    public class Bbox
    {
        [DataMember(Name = "east")]
        public double East { get; set; }
        [DataMember(Name = "south")]
        public double South { get; set; }
        [DataMember(Name = "north")]
        public double North { get; set; }
        [DataMember(Name = "west")]
        public double West { get; set; }
    }
}