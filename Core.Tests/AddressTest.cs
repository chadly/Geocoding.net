using System;
using Xunit;

namespace GeoCoding.Tests.AddressSpecs
{
    public class AddressTest
    {
        [Fact]
        public void CanCreateWithDefaultValues()
        {
            Address a = new Address();

            Assert.Equal("", a.Street);
            Assert.Equal("", a.City);
            Assert.Equal("", a.State);
            Assert.Equal("", a.PostalCode);
            Assert.Equal("", a.Country);

            Assert.Equal(Location.Empty, a.Coordinates);
            Assert.Equal(AddressAccuracy.Unknown, a.Accuracy);
        }

        [Fact]
        public void CanChangeAddressLocation()
        {
            Address a = new Address();
            Location loc = new Location(57.68, 43.23);

            a.ChangeLocation(loc, AddressAccuracy.CityLevel);

            Assert.Equal(loc, a.Coordinates);
            Assert.Equal(AddressAccuracy.CityLevel, a.Accuracy);
        }

        [Fact]
        public void CanCalculateHaversineDistanceBetweenTwoAddresses()
        {
            Address address1 = new Address();
            address1.ChangeLocation(new Location(0, 0), AddressAccuracy.Unknown);

            Address address2 = new Address();
            address2.ChangeLocation(new Location(40, 20), AddressAccuracy.Unknown);

            Distance distance1 = address1.DistanceBetween(address2);
            Distance distance2 = address2.DistanceBetween(address1);

            Assert.Equal(distance1, distance2);
        }
    }
}
