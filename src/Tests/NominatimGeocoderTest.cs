using Geocoding;
using Geocoding.Nominatim;
using Geocoding.Tests;
using Microsoft.Framework.Configuration;

namespace Tests
{
    public class NominatimGeocoderTest : GeocoderTest
    {
        private IGeocoder geocoder;

        protected override IGeocoder CreateGeocoder()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string email = config["AppSettings:nominatimEmail"];

            geocoder = new NominatimGeocoder() { Email = email };

            return geocoder;
        }
    }
}