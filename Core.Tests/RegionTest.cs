using System;
using Xunit;
using XunitExt;

namespace GeoCoding.Tests
{
	public class RegionTest
	{
		[Fact]
		public void CanCreateWithDefaultValues()
		{
			Region region = new Region();

			Assert.Equal("", region.PostalCode);
			Assert.Equal("", region.City);
			Assert.Equal("", region.State);
			Assert.Equal("", region.Country);
			Assert.Equal(Location.Empty, region.Coordinates);
			Assert.Equal(RegionLevel.Unknown, region.Level);
		}

		[Fact]
		public void CanCreate()
		{
			Location loc = new Location(95, 90);
			Region region = new Region("US", "CA", "San Francisco", "12345", loc, RegionLevel.PostalCodeLevel);

			Assert.Equal("US", region.Country);
			Assert.Equal("CA", region.State);
			Assert.Equal("San Francisco", region.City);
			Assert.Equal("12345", region.PostalCode);
			Assert.Equal(loc, region.Coordinates);
			Assert.Equal(RegionLevel.PostalCodeLevel, region.Level);
		}

		[Theory]
		[InlineData(AddressAccuracy.CountryLevel, RegionLevel.CountryLevel)]
		[InlineData(AddressAccuracy.StateLevel, RegionLevel.StateLevel)]
		[InlineData(AddressAccuracy.CityLevel, RegionLevel.CityLevel)]
		[InlineData(AddressAccuracy.PostalCodeLevel, RegionLevel.PostalCodeLevel)]
		[InlineData(AddressAccuracy.StreetLevel, RegionLevel.PostalCodeLevel)]
		[InlineData(AddressAccuracy.AddressLevel, RegionLevel.PostalCodeLevel)]
		[InlineData(AddressAccuracy.Unknown, RegionLevel.Unknown)]
		public void CanCreateFromAddress(AddressAccuracy accuracy, RegionLevel level)
		{
			Address address = new Address("123 Main St.", "San Francisco", "CA", "12345", "US",
				new Location(123, 22), accuracy);

			Region region = address;
			Assert.Equal(address.Country, region.Country);
			Assert.Equal(address.State, region.State);
			Assert.Equal(address.City, region.City);
			Assert.Equal(address.PostalCode, region.PostalCode);
			Assert.Equal(address.Coordinates, region.Coordinates);
			Assert.Equal(level, region.Level);
		}

		[Fact]
		public void CanCompareForEquality()
		{
			Location loc = new Location(23, 43);
			Region r1 = new Region("US", "CA", "San Francisco", "12345", loc, RegionLevel.PostalCodeLevel);
			Region r2 = new Region("US", "CA", "San Francisco", "12345", loc, RegionLevel.PostalCodeLevel);

			Assert.True(r1.Equals(r2));
			Assert.Equal(r1.GetHashCode(), r2.GetHashCode());
		}

		[Fact]
		public void CanCompareForEqualityWithNullValues()
		{
			Region r1 = new Region("", "", "", "", Location.Empty, RegionLevel.Unknown);
			Region r2 = new Region(null, null, null, null, Location.Empty, RegionLevel.Unknown);

			Assert.True(r1.Equals(r2));
			Assert.Equal(r1.GetHashCode(), r2.GetHashCode());
		}

		[Fact]
		public void CanCreateEmpty()
		{
			Region region = new Region();
			Assert.Equal(Region.Empty, region);
		}

		[Theory]
		[InlineData(RegionLevel.CountryLevel, "US")]
		[InlineData(RegionLevel.StateLevel, "CA, US")]
		[InlineData(RegionLevel.CityLevel, "San Francisco, CA, US")]
		[InlineData(RegionLevel.PostalCodeLevel, "San Francisco, CA 12345, US")]
		[InlineData(RegionLevel.Unknown, "San Francisco, CA 12345, US")]
		public void CanFormatToString(RegionLevel level, string str)
		{
			Region region = new Region("US", "CA", "San Francisco", "12345", Location.Empty, level);
			Assert.Equal(str, region.ToString());
		}

		[Theory]
		[InlineData("US", true)]
		[InlineData("CA", false)]
		[InlineData(null, false)]
		public void CanAssertAddressWithinCountryLevel(string country, bool contains)
		{
			Region region = new Region("US", null, null, null, Location.Empty, RegionLevel.CountryLevel);
			Address address = new Address("123 Main St.", "Anytown", "CA", "12345", country);
			Assert.Equal(contains, region.Contains(address));
		}

		[Theory]
		[InlineData("US", "CA", true)]
		[InlineData("CA", "ON", false)]
		[InlineData("US", "LA", false)]
		[InlineData(null, null, false)]
		public void CanAssertAddressWithinStateLevel(string country, string state, bool contains)
		{
			Region region = new Region("US", "CA", null, null, Location.Empty, RegionLevel.StateLevel);
			Address address = new Address("123 Main St.", "Anytown", state, "12345", country);
			Assert.Equal(contains, region.Contains(address));
		}

		[Theory]
		[InlineData("US", "CA", "San Francisco", true)]
		[InlineData("US", "LA", "San Francisco", false)]
		[InlineData("CA", "US", "San Francisco", false)]
		[InlineData("US", "CA", "New Orleans", false)]
		[InlineData(null, null, null, false)]
		public void CanAssertAddressWithinCityLevel(string country, string state, string city, bool contains)
		{
			Region region = new Region("US", "CA", "San Francisco", null, Location.Empty, RegionLevel.CityLevel);
			Address address = new Address("123 Main St.", city, state, "12345", country);
			Assert.Equal(contains, region.Contains(address));
		}

		[Theory]
		[InlineData("US", "CA", "San Francisco", "12345", true)]
		[InlineData("US", "CA", "San Francisco", "54321", false)]
		[InlineData("US", "LA", "San Francisco", "12345", false)]
		[InlineData("CA", "US", "San Francisco", "12345", false)]
		[InlineData("US", "CA", "New Orleans", "12345", false)]
		[InlineData(null, null, null, null, false)]
		public void CanAssertAddressWithinPostalCodeLevel(string country, string state, string city, string postalCode, bool contains)
		{
			Region region = new Region("US", "CA", "San Francisco", "12345", Location.Empty, RegionLevel.PostalCodeLevel);
			Address address = new Address("123 Main St.", city, state, postalCode, country);
			Assert.Equal(contains, region.Contains(address));
		}
	}
}
