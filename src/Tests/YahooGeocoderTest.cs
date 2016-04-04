using Geocoding.Yahoo;
using Microsoft.Framework.Configuration;
using Xunit;

namespace Geocoding.Tests
{
    public class YahooGeocoderTest : GeocoderTest
    {
        protected override IGeocoder CreateGeocoder()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            return new YahooGeocoder(
                config["AppSettings:yahooConsumerKey"],
                config["AppSettings:yahooConsumerSecret"]
            );
        }

        //TODO: delete these when tests are ready to be unskipped
        //see issue #27

        [Fact(Skip = "oauth not working for yahoo - see issue #27")]
        public override void CanGeocodeAddress(string address) { }

        [Fact(Skip = "oauth not working for yahoo - see issue #27")]
        public override void CanGeocodeNormalizedAddress() { }

        [Fact(Skip = "oauth not working for yahoo - see issue #27")]
        public override void CanGeocodeAddressUnderDifferentCultures(string cultureName) { }

        [Fact(Skip = "oauth not working for yahoo - see issue #27")]
        public override void CanReverseGeocodeAddressUnderDifferentCultures(string cultureName) { }

        [Fact(Skip = "oauth not working for yahoo - see issue #27")]
        public override void ShouldNotBlowUpOnBadAddress() { }

        [Fact(Skip = "oauth not working for yahoo - see issue #27")]
        public override void CanGeocodeWithSpecialCharacters(string address) { }

        [Fact(Skip = "oauth not working for yahoo - see issue #27")]
        public override void CanReverseGeocode() { }

        [Fact(Skip = "oauth not working for yahoo - see issue #27")]
        public override void CanGeocodeInvalidZipCodes(string address) { }
    }
}