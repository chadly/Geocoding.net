using System;
using Xunit;

namespace GeoCoding.Tests
{
    public class AddressTest
    {
        [Fact]
        public void CanCreateEmpty()
        {
            Address address = new Address();
            Assert.Equal(Address.Empty, address);
        }

        [Fact]
        public void CanCreate()
        {
            string street = "123 Main St.";
            string city = "AnyTown";
            string state = "CA";
            string postalCode = "12345";
            Country country = Country.US;

            Address a = new Address(street, city, state, postalCode, country);

            Assert.Equal(street, a.Street);
            Assert.Equal(city, a.City);
            Assert.Equal(state, a.State);
            Assert.Equal(postalCode, a.PostalCode);
            Assert.Equal(country, a.Country);

            Assert.Equal(Location.Empty, a.Coordinates);
            Assert.Equal(AddressAccuracy.Unknown, a.Accuracy);
        }

        [Fact]
        public void CanCompareForEquality()
        {
            Address address1 = new Address("123 Main Street", "Anytown", "CA", "12345", Country.US);
            Address address2 = new Address("123 Main Street", "Anytown", "CA", "12345", Country.US);

            Assert.True(address1.Equals(address2));
            Assert.Equal(address1.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void CanNotHaveNullValues()
        {
            Address address = new Address();

            Assert.NotNull(address.Street);
            Assert.NotNull(address.City);
            Assert.NotNull(address.State);
            Assert.NotNull(address.PostalCode);
            Assert.NotNull(address.Country);
        }

        [Fact]
        public void CanCalculateHaversineDistanceBetweenTwoAddresses()
        {
            Address address1 = new Address(null, null, null, null, Country.Unspecified, new Location(0, 0), AddressAccuracy.Unknown);
            Address address2 = new Address(null, null, null, null, Country.Unspecified, new Location(40, 20), AddressAccuracy.Unknown);

            Distance distance1 = address1.DistanceBetween(address2);
            Distance distance2 = address2.DistanceBetween(address1);

            Assert.Equal(distance1, distance2);
        }
    }
}
