using System.Globalization;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Extensions;

namespace GeoCoding.Tests
{
	public abstract class GeoCoderTest
	{
		readonly IGeoCoder geoCoder;

		public GeoCoderTest()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");

			geoCoder = CreateGeoCoder();
		}

		protected abstract IGeoCoder CreateGeoCoder();

		[Fact]
		public void CanGeoCodeAddress()
		{
			Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave washington dc").ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Fact]
		public void CanGeoCodeNormalizedAddress()
		{
			Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave", "washington", "dc", null, null).ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanGeoCodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = geoCoder.GeoCode("24 sussex drive ottawa, ontario").ToArray();
			addresses[0].AssertCanadianPrimeMinister();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanReverseGeoCodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = geoCoder.ReverseGeocode(38.8976777, -77.036517).ToArray();
			addresses[0].AssertWhiteHouseArea();
		}

		[Fact]
		public void ShouldNotBlowUpOnBadAddress()
		{
			var addresses = geoCoder.GeoCode("sdlkf;jasl;kjfldksjfasldf");
			Assert.Empty(addresses);
		}

		[Fact]
		public void CanGeoCodeWithSpecialCharacters()
		{
			var addresses = geoCoder.GeoCode("Fried St & 2nd St, Gretna, LA 70053");

			//asserting no exceptions are thrown and that we get something
			Assert.NotEmpty(addresses);
		}

		[Fact]
		public void CanReverseGeoCode()
		{
			Address[] addresses = geoCoder.ReverseGeocode(38.8976777, -77.036517).ToArray();
			addresses[0].AssertWhiteHouseArea();
		}

		[Theory]
		[InlineData("1 Robert Wood Johnson Hosp New Brunswick, NJ 08901 USA")]
		[InlineData("miss, MO")]
		//https://github.com/chadly/Geocoding.net/issues/6
		public void CanGeoCodeInvalidZipCodes(string address)
		{
			Address[] addresses = geoCoder.GeoCode(address).ToArray();
			Assert.NotEmpty(addresses);
		}
	}
}