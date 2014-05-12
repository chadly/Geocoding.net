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

		[Theory]
		[InlineData("1600 pennsylvania ave nw, washington dc")]
		public virtual void CanGeocodeAddress(string address)
		{
			Address[] addresses = geocoder.Geocode(address).ToArray();
			addresses[0].AssertWhiteHouse();
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
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = geocoder.Geocode("24 sussex drive ottawa, ontario").ToArray();
			addresses[0].AssertCanadianPrimeMinister();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public virtual void CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

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
		[InlineData("Fried St & 2nd St, Gretna, LA 70053")]
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
		[InlineData("1 Robert Wood Johnson Hosp New Brunswick, NJ 08901 USA")]
		[InlineData("miss, MO")]
		//https://github.com/chadly/Geocoding.net/issues/6
		public virtual void CanGeocodeInvalidZipCodes(string address)
		{
			Address[] addresses = geocoder.Geocode(address).ToArray();
			Assert.NotEmpty(addresses);
		}
	}
}