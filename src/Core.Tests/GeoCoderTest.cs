using System;
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

		private void AssertWhiteHouseAddress(Address address)
		{
			Assert.True("The White House".Equals(address.Street) || "1600 Pennsylvania Ave NW".Equals(address.Street));
			Assert.Equal("Washington", address.City);
			Assert.Equal("DC", address.State);
			Assert.True(string.IsNullOrEmpty(address.PostalCode) || "20006".Equals(address.PostalCode) || "20500".Equals(address.PostalCode));
			Assert.Equal(AddressAccuracy.AddressLevel, address.Accuracy);
			Assert.True(address.Country == "US" || address.Country == "United States");
		}

		[Fact]
		public void CanGeoCodeAddress()
		{
			Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave washington dc");
			AssertWhiteHouseAddress(addresses[0]);
		}

		[Fact]
		public void CanGeoCodeNormalizedAddress()
		{
			Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave", "washington", "dc", null, null);
			AssertWhiteHouseAddress(addresses[0]);
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanGeoCodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = geoCoder.GeoCode("24 sussex drive ottawa, ontario");

			Assert.Equal("24 Sussex Dr", addresses[0].Street);
			Assert.Equal("Ottawa", addresses[0].City);
			Assert.Equal("ON", addresses[0].State);
			Assert.True(addresses[0].PostalCode.StartsWith("K1M"));
			Assert.True(addresses[0].Country == "CA" || addresses[0].Country == "Canada");
			Assert.Equal(AddressAccuracy.AddressLevel, addresses[0].Accuracy);
		}

		[Theory]
		[InlineData("United States", AddressAccuracy.CountryLevel)]
		[InlineData("Illinois, US", AddressAccuracy.StateLevel)]
		[InlineData("New York, New York", AddressAccuracy.CityLevel)]
		[InlineData("90210, US", AddressAccuracy.PostalCodeLevel)]
		[InlineData("1600 pennsylvania ave washington dc", AddressAccuracy.AddressLevel)]
		public void CanMatchAccuracyLevelsOfAddress(string address, AddressAccuracy accuracy)
		{
			Address[] addresses = geoCoder.GeoCode(address);
			Assert.Equal(accuracy, addresses[0].Accuracy);
		}
	}
}