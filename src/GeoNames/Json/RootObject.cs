using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Geocoding.GeoNames.Json
{
    public class RootObject
    {
        [DataMember(Name = "totalResultsCount", IsRequired = false)]
        public int TotalResultsCount { get; set; }
        [DataMember(Name = "geonames", IsRequired = true, EmitDefaultValue = false)]
        public List<Geoname> Geonames { get; set; }
    }
}