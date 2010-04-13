using System;
using Xunit;

namespace GeoCoding.Tests.AddressSpecs
{
    public class AddressTest
    {
        [Fact]
        public void CanCreateWithDefaultValues()
        {
			Address a = new Address(null, null, null, null, null, null, AddressAccuracy.Unknown);

            Assert.Equal("", a.Street);
            Assert.Equal("", a.City);
            Assert.Equal("", a.State);
            Assert.Equal("", a.PostalCode);
            Assert.Equal("", a.Country);
			Assert.Equal(null, a.Coordinates);
            Assert.Equal(AddressAccuracy.Unknown, a.Accuracy);
        }
    }
}
