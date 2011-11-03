using System;
using System.Linq;
using System.Globalization;
using System.Threading;
using Xunit;
using Xunit.Extensions;

namespace GeoCoding.Tests
{
	public abstract class GeoCoderTest
	{
		private readonly IGeoCoder geoCoder;

		public GeoCoderTest()
		{
			geoCoder = CreateGeoCoder();
		}

		protected abstract IGeoCoder CreateGeoCoder();

		[Fact]
		public void CanGeoCodeAddress()
		{
			Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave washington dc").ToArray();
			AssertWhiteHouseAddress(addresses[0]);
		}

		[Fact]
		public void CanGeoCodeNormalizedAddress()
		{
			Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave", "washington", "dc", null, null).ToArray();
			AssertWhiteHouseAddress(addresses[0]);
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanGeoCodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = geoCoder.GeoCode("24 sussex drive ottawa, ontario").ToArray();

			Address addr = addresses[0];

			Assert.True(addr.FormattedAddress.Contains("24 Sussex"));
			Assert.True(addr.FormattedAddress.Contains("Ottawa, ON"));
			Assert.True(addr.FormattedAddress.Contains("K1M"));
			Assert.True(addr.FormattedAddress.Contains("CA") || addr.FormattedAddress.Contains("Canada"));
		}

		//[Theory]
		//[InlineData("United States", AddressAccuracy.CountryLevel)]
		//[InlineData("Illinois, US", AddressAccuracy.StateLevel)]
		//[InlineData("New York, New York", AddressAccuracy.CityLevel)]
		//[InlineData("90210, US", AddressAccuracy.PostalCodeLevel)]
		//[InlineData("1600 pennsylvania ave washington dc", AddressAccuracy.AddressLevel)]
		//public void CanMatchAccuracyLevelsOfAddress(string address, AddressAccuracy accuracy)
		//{
		//    Address[] addresses = geoCoder.GeoCode(address);
		//    Assert.Equal(accuracy, addresses[0].Accuracy);
		//}

		private void AssertWhiteHouseAddress(Address address)
		{
			Assert.True(address.FormattedAddress.Contains("The White House") || address.FormattedAddress.Contains("1600 Pennsylvania Ave NW"));
			Assert.True(address.FormattedAddress.Contains("Washington, DC"));
			Assert.True(address.FormattedAddress.Contains("US") || address.FormattedAddress.Contains("United States"));

			Assert.Equal(38.8976777, address.Coordinates.Latitude);
			Assert.Equal(-77.0365170, address.Coordinates.Longitude);
		}
	}
}