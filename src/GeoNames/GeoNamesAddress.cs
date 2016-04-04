using System.Collections.Generic;
using Geocoding;
using Geocoding.GeoNames.Json;

namespace Geocoding.GeoNames
{
    internal class GeoNamesAddress : Address
    {
        public GeoNamesAddress(string formattedAddress, Location coordinates) : base(formattedAddress, coordinates, "GeoNames")
        {
        }

        public string ASCII { get; set; }
        public string AdminCode1 { get; set; }
        public string AdminCode2 { get; set; }
        public string AdminCode3 { get; set; }
        public string AdminId1 { get; set; }
        public string AdminId2 { get; set; }
        public string AdminId3 { get; set; }
        public string AdminName1 { get; set; }
        public string AdminName2 { get; set; }
        public string AdminName3 { get; set; }
        public string AdminName4 { get; set; }
        public string AdminName5 { get; set; }
        public List<AlternateName> AlternateNames { get; set; }
        public Bbox BoundingBox { get; set; }
        public string ContinentCode { get; set; }
        public string CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public int GeonameId { get; set; }
        public string Fcl { get; set; }
        public string FclName { get; set; }
        public string Fcode { get; set; }
        public string FcodeName { get; set; }
        public int Population { get; set; }
        public double Score { get; set; }
        public Timezone Timezone { get; set; }
        public string ToponymName { get; set; }
    }
}