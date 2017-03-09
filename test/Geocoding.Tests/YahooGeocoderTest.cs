using Geocoding.Yahoo;
using System.Threading.Tasks;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class YahooGeocoderTest : GeocoderTest
	{
		public YahooGeocoderTest(SettingsFixture settings)
			: base(settings) { }

		protected override IGeocoder CreateGeocoder()
		{
			return new YahooGeocoder(
				settings.YahooConsumerKey,
				settings.YahooConsumerSecret
			);
		}

		//TODO: delete these when tests are ready to be unskipped
		//see issue #27

		[Fact(Skip = "oauth not working for yahoo - see issue #27")]
		public override Task CanGeocodeAddress(string address)
		{
		    return Task.CompletedTask;
		}

		[Fact(Skip = "oauth not working for yahoo - see issue #27")]
		public override Task CanGeocodeNormalizedAddress()
        {
            return Task.CompletedTask;
        }

		[Fact(Skip = "oauth not working for yahoo - see issue #27")]
		public override Task CanGeocodeAddressUnderDifferentCultures(string cultureName)
        {
            return Task.CompletedTask;
        }

		[Fact(Skip = "oauth not working for yahoo - see issue #27")]
		public override Task CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
        {
            return Task.CompletedTask;
        }

		[Fact(Skip = "oauth not working for yahoo - see issue #27")]
		public override Task ShouldNotBlowUpOnBadAddress()
        {
            return Task.CompletedTask;
        }

		[Fact(Skip = "oauth not working for yahoo - see issue #27")]
		public override Task CanGeocodeWithSpecialCharacters(string address)
        {
            return Task.CompletedTask;
        }

		[Fact(Skip = "oauth not working for yahoo - see issue #27")]
		public override Task CanReverseGeocodeAsync()
        {
            return Task.CompletedTask;
        }

		[Fact(Skip = "oauth not working for yahoo - see issue #27")]
		public override Task CanGeocodeInvalidZipCodes(string address)
        {
            return Task.CompletedTask;
        }

		[Fact(Skip = "oauth not working for yahoo - see issue #27")]
		public override Task CanHandleStreetIntersectionsByAmpersand(string address)
        {
            return Task.CompletedTask;
        }
	}
}
