using System.Globalization;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Extensions;

namespace Geocoding.Tests
{
	public abstract class GeocoderTest
	{
		readonly IGeocoder geocoder;

		public GeocoderTest()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");

			geocoder = CreateGeocoder();
		}

		protected abstract IGeocoder CreateGeocoder();

		[Fact]
		public void CanGeocodeAddress()
		{
			Address[] addresses = geocoder.Geocode("1600 pennsylvania ave washington dc").ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Fact]
		public void CanGeocodeNormalizedAddress()
		{
			Address[] addresses = geocoder.Geocode("1600 pennsylvania ave", "washington", "dc", null, null).ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = geocoder.Geocode("24 sussex drive ottawa, ontario").ToArray();
			addresses[0].AssertCanadianPrimeMinister();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = geocoder.ReverseGeocode(38.8976777, -77.036517).ToArray();
			addresses[0].AssertWhiteHouseArea();
		}

		[Fact]
		public void ShouldNotBlowUpOnBadAddress()
		{
			var addresses = geocoder.Geocode("sdlkf;jasl;kjfldksjfasldf");
			Assert.Empty(addresses);
		}

		[Fact]
		public void CanGeocodeWithSpecialCharacters()
		{
			var addresses = geocoder.Geocode("Fried St & 2nd St, Gretna, LA 70053");

			//asserting no exceptions are thrown and that we get something
			Assert.NotEmpty(addresses);
		}

		[Fact]
		public void CanReverseGeocode()
		{
			Address[] addresses = geocoder.ReverseGeocode(38.8976777, -77.036517).ToArray();
			addresses[0].AssertWhiteHouseArea();
		}

		[Theory]
		[InlineData("1 Robert Wood Johnson Hosp New Brunswick, NJ 08901 USA")]
		[InlineData("miss, MO")]
		//https://github.com/chadly/Geocoding.net/issues/6
		public void CanGeocodeInvalidZipCodes(string address)
		{
			Address[] addresses = geocoder.Geocode(address).ToArray();
			Assert.NotEmpty(addresses);
		}
	}
}