using System;
using Xunit;

namespace GeoCoding.Services.Tests
{
    public abstract class GeoCoderTest
    {
        private readonly IGeoCoder _geoCoder;

        public GeoCoderTest()
        {
            _geoCoder = CreateGeoCoder();
        }

        protected abstract IGeoCoder CreateGeoCoder();

        private void AssertWhiteHouseAddress(Address address)
        {
            Assert.Equal("1600 Pennsylvania Ave NW", address.Street);
            Assert.Equal("Washington", address.City);
            Assert.Equal("DC", address.State);
            Assert.Equal("20006", address.PostalCode);
            Assert.Equal(Country.US, address.Country);
            Assert.Equal(AddressAccuracy.AddressLevel, address.Accuracy);
        }

        [Fact]
        public void CanGeoCodeAddress()
        {
            Address[] addresses = _geoCoder.GeoCode("1600 pennsylvania ave washington dc");

            Assert.Equal(1, addresses.Length);
            AssertWhiteHouseAddress(addresses[0]);
        }

        [Fact]
        public void CanGeoCodeNormalizedAddress()
        {
            Address[] addresses = _geoCoder.GeoCode("1600 pennsylvania ave", "washington", "dc", null, Country.Unspecified);

            Assert.Equal(1, addresses.Length);
            AssertWhiteHouseAddress(addresses[0]);
        }

        [Fact]
        public void CanValidateAddress()
        {
            Address invalidAddress = new Address("1600 pennsylvania ave", "washington", "dc", null, Country.Unspecified);

            Address[] addresses = _geoCoder.Validate(invalidAddress);

            Assert.Equal(1, addresses.Length);
            AssertWhiteHouseAddress(addresses[0]);
        }
    }
}
