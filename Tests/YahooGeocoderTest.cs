using System.Configuration;
using Geocoding.Yahoo;
using Xunit;

namespace Geocoding.Tests
{
	public class YahooGeocoderTest : GeocoderTest
	{
		protected override IGeocoder CreateGeocoder()
		{
			return new YahooGeocoder(
				ConfigurationManager.AppSettings["yahooConsumerKey"],
				ConfigurationManager.AppSettings["yahooConsumerSecret"]
			);
		}

		//TODO: delete these when tests are ready to be unskipped

		[Fact(Skip = "oauth not working for yahoo")]
		public override void CanGeocodeAddress(string address) { }

		[Fact(Skip = "oauth not working for yahoo")]
		public override void CanGeocodeNormalizedAddress() { }

		[Fact(Skip = "oauth not working for yahoo")]
		public override void CanGeocodeAddressUnderDifferentCultures(string cultureName) { }

		[Fact(Skip = "oauth not working for yahoo")]
		public override void CanReverseGeocodeAddressUnderDifferentCultures(string cultureName) { }

		[Fact(Skip = "oauth not working for yahoo")]
		public override void ShouldNotBlowUpOnBadAddress() { }

		[Fact(Skip = "oauth not working for yahoo")]
		public override void CanGeocodeWithSpecialCharacters(string address) { }

		[Fact(Skip = "oauth not working for yahoo")]
		public override void CanReverseGeocode() { }

		[Fact(Skip = "oauth not working for yahoo")]
		public override void CanGeocodeInvalidZipCodes(string address) { }
	}
}