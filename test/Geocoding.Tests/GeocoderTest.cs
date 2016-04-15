using System.Globalization;
using System.Linq;
using Xunit;

namespace Geocoding.Tests
{
	public abstract class GeocoderTest
	{
		readonly IGeocoder geocoder;

		public GeocoderTest()
		{
			CultureInfo.CurrentCulture = new CultureInfo("en-us");

			geocoder = CreateGeocoder();
		}

		protected abstract IGeocoder CreateGeocoder();

		[Theory]
		[InlineData("1600 pennsylvania ave nw, washington dc")]
		public virtual void CanGeocodeAddress(string address)
		{
			Address[] addresses = geocoder.Geocode(address).ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Theory]
		[InlineData("10557 platz der republik, berlin")]
		public virtual void CanGeoCodeGermanAddress(string address)
		{
			Address[] addresses = geocoder.Geocode(address).ToArray();
			addresses[0].AssertReichstag();
		}

		[Fact]
		public virtual void CanGeocodeNormalizedAddress()
		{
			Address[] addresses = geocoder.Geocode("1600 pennsylvania ave nw", "washington", "dc", null, null).ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public virtual void CanGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			CultureInfo.CurrentCulture = new CultureInfo("en-us");

            Address[] addresses = geocoder.Geocode("24 sussex drive ottawa, ontario").ToArray();
			addresses[0].AssertCanadianPrimeMinister();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public virtual void CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			CultureInfo.CurrentCulture = new CultureInfo("en-us");

            Address[] addresses = geocoder.ReverseGeocode(38.8976777, -77.036517).ToArray();
			addresses[0].AssertWhiteHouseArea();
		}

		[Fact]
		public virtual void ShouldNotBlowUpOnBadAddress()
		{
			Address[] addresses = geocoder.Geocode("sdlkf;jasl;kjfldksj,fasldf").ToArray();
			Assert.Empty(addresses);
		}

		[Theory]
		[InlineData("Wilshire & Bundy, Los Angeles")]
		public virtual void CanGeocodeWithSpecialCharacters(string address)
		{
			Address[] addresses = geocoder.Geocode(address).ToArray();

			//asserting no exceptions are thrown and that we get something
			Assert.NotEmpty(addresses);
		}

		[Fact]
		public virtual void CanReverseGeocode()
		{
			Address[] addresses = geocoder.ReverseGeocode(38.8976777, -77.036517).ToArray();
			addresses[0].AssertWhiteHouseArea();
		}

		[Theory]
		[InlineData("Robert Wood Johnson University Hospital, New Brunswick, NJ 08901 USA")]
		[InlineData("miss, MO")]
		//https://github.com/chadly/Geocoding.net/issues/6
		public virtual void CanGeocodeInvalidZipCodes(string address)
		{
			Address[] addresses = geocoder.Geocode(address).ToArray();
			Assert.NotEmpty(addresses);
		}
	}
}