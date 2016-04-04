using Geocoding;
using Geocoding.GeoNames;
using Geocoding.Nominatim;
using Geocoding.Tests;
using Microsoft.Framework.Configuration;

namespace Tests
{
    public class GeoNamesGeocoderTest : GeocoderTest
    {
        private IGeocoder geocoder;

        protected override IGeocoder CreateGeocoder()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string userName = config["AppSettings:geoNamesUserName"];

            geocoder = new GeoNamesGeocoder() { UserName = userName };

            return geocoder;
        }
    }
}