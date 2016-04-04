using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Geocoding.GeoNames.Json
{
    [DataContract]
    public class Geoname
    {
        [DataMember(Name = "timezone", IsRequired = false)]
        public Timezone Timezone { get; set; }
        [DataMember(Name = "bbox", IsRequired =false)]
        public Bbox BoundingBox { get; set; }
        [DataMember(Name = "asciiName", IsRequired =false)]
        public string ASCIIName { get; set; }
        [DataMember(Name = "countryId", IsRequired =true)]
        public string CountryId { get; set; }
        [DataMember(Name = "fcl", IsRequired =false)]
        public string Fcl { get; set; }
        [DataMember(Name = "score", IsRequired =false)]
        public double Score { get; set; }
        [DataMember(Name = "adminId2", IsRequired =false)]
        public string AdminId2 { get; set; }
        [DataMember(Name = "countryCode", IsRequired =false)]
        public string CountryCode { get; set; }
        [DataMember(Name = "adminId1", IsRequired =false)]
        public string AdminId1 { get; set; }
        [DataMember(Name = "lat", IsRequired = false)]
        public double Lat { get; set; }
        [DataMember(Name = "fcode", IsRequired = false)]
        public string Fcode { get; set; }
        [DataMember(Name = "continentCode", IsRequired = false)]
        public string ContinentCode { get; set; }
        [DataMember(Name = "adminCode2", IsRequired = false)]
        public string AdminCode2 { get; set; }
        [DataMember(Name = "adminCode1", IsRequired = false)]
        public string AdminCode1 { get; set; }
        [DataMember(Name = "lng", IsRequired = false)]
        public double Lng { get; set; }
        [DataMember(Name = "geonameId", IsRequired = false)]
        public int GeonameId { get; set; }
        [DataMember(Name = "toponymName", IsRequired = false)]
        public string ToponymName { get; set; }
        [DataMember(Name = "population", IsRequired = false)]
        public int Population { get; set; }
        [DataMember(Name = "adminName5", IsRequired = false)]
        public string AdminName5 { get; set; }
        [DataMember(Name = "adminName4", IsRequired = false)]
        public string AdminName4 { get; set; }
        [DataMember(Name = "adminName3", IsRequired = false)]
        public string AdminName3 { get; set; }
        [DataMember(Name = "alternateNames", IsRequired = false)]
        public List<AlternateName> AlternateNames { get; set; }
        [DataMember(Name = "adminName2", IsRequired = false)]
        public string AdminName2 { get; set; }
        [DataMember(Name = "name", IsRequired = false)]
        public string Name { get; set; }
        [DataMember(Name = "fclName", IsRequired = false)]
        public string FclName { get; set; }
        [DataMember(Name = "countryName", IsRequired = false)]
        public string CountryName { get; set; }
        [DataMember(Name = "fcodeName", IsRequired = false)]
        public string FcodeName { get; set; }
        [DataMember(Name = "adminName1", IsRequired = false)]
        public string AdminName1 { get; set; }
        [DataMember(Name = "adminId3", IsRequired = false)]
        public string AdminId3 { get; set; }
        [DataMember(Name = "adminCode3", IsRequired = false)]
        public string AdminCode3 { get; set; }
        [DataMember(Name = "distance", IsRequired = false)]
        public double Distance { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            
            sb.AddIfNotNullOrEmpty(CountryName, ", ");

            sb.AddIfNotNullOrEmpty(AdminName1, ", ");
            sb.AddIfNotNullOrEmpty(AdminName2, ", ");
            sb.AddIfNotNullOrEmpty(AdminName3, ", ");
            sb.AddIfNotNullOrEmpty(AdminName4, ", ");
            sb.AddIfNotNullOrEmpty(AdminName5, ", ");

            sb.AddIfNotNullOrEmpty(Name, ", ");
            
            return sb.ToString().TrimEnd(',',' ');
        }
    }
}