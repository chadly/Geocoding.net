using Geocoding.Yahoo;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class YahooGeocoderTest : GeocoderTest
	{
		readonly SettingsFixture settings;

		public YahooGeocoderTest(SettingsFixture settings)
		{
			this.settings = settings;
		}

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

		[Fact(Skip = "oauth not working for yahoo - see issue #27")]
		public override void CanHandleStreetIntersectionsByAmpersand(string address) { }
	}
}
