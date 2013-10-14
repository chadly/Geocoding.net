using System;
using Xunit;

namespace GeoCoding.Tests
{
	public static class AddressAssertionExtensions
	{
		public static void AssertWhiteHouse(this Address address)
		{
			Assert.True(address.FormattedAddress.Contains("The White House") || address.FormattedAddress.Contains("1600 Pennsylvania Ave NW"));
			AssertWhiteHouseArea(address);
		}

		public static void AssertWhiteHouseArea(this Address address)
		{
			Assert.True(address.FormattedAddress.Contains("Washington, DC"));

			//just hoping that each geocoder implementation gets it somewhere near the vicinity
			double lat = Math.Round(address.Coordinates.Latitude, 2);
			Assert.Equal(38.90, lat);

			double lng = Math.Round(address.Coordinates.Longitude, 2);
			Assert.Equal(-77.04, lng);
		}

		public static void AssertCanadianPrimeMinister(this Address address)
		{
			Assert.True(address.FormattedAddress.Contains("24 Sussex"));
			Assert.True(address.FormattedAddress.Contains("Ottawa, ON"));
			Assert.True(address.FormattedAddress.Contains("K1M"));
		}
	}
}