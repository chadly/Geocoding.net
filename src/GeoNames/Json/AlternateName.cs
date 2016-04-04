using System.Runtime.Serialization;

namespace Geocoding.GeoNames.Json
{
    [DataContract]
    public class AlternateName
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "lang")]
        public string Language { get; set; }
    }
}