using System.Runtime.Serialization;

namespace Geocoding.GeoNames.Json
{
    [DataContract]
    public class Timezone
    {
        [DataMember(Name = "gmtOffset")]
        public int GmtOffset { get; set; }
        [DataMember(Name = "timeZoneId")]
        public string TimeZoneId { get; set; }

        [DataMember(Name = "dstOffset")]
        public int DstOffset { get; set; }
    }
}