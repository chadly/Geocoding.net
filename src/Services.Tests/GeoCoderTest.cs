using System;
using System.Threading;
using System.Globalization;
using Xunit;
using Xunit.Extensions;

namespace GeoCoding.Services.Tests
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
            Assert.Equal("1600 Pennsylvania Ave NW", address.Street);
            Assert.Equal("Washington", address.City);
            Assert.Equal("DC", address.State);
            Assert.Equal("20006", address.PostalCode);
            Assert.Equal("US", address.Country);
            Assert.Equal(AddressAccuracy.AddressLevel, address.Accuracy);
        }

        [Fact]
        public void CanGeoCodeAddress()
        {
            Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave washington dc");

            Assert.Equal(1, addresses.Length);
            AssertWhiteHouseAddress(addresses[0]);
        }

        [Fact]
        public void CanGeoCodeNormalizedAddress()
        {
            Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave", "washington", "dc", null, null);

            Assert.Equal(1, addresses.Length);
            AssertWhiteHouseAddress(addresses[0]);
        }

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanGeoCodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = geoCoder.GeoCode("24 sussex drive ottawa, ontario");

			Assert.Equal(1, addresses.Length);
			Assert.Equal("24 Sussex Dr", addresses[0].Street);
			Assert.Equal("Ottawa", addresses[0].City);
			Assert.Equal("ON", addresses[0].State);
			Assert.Equal("K1M", addresses[0].PostalCode);
			Assert.Equal("CA", addresses[0].Country);
			Assert.Equal(AddressAccuracy.AddressLevel, addresses[0].Accuracy);
		}
    }
}
