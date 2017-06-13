using System;
using Xunit;

namespace Geocoding.Tests
{
	public static class AddressAssertionExtensions
	{
		public static void AssertWhiteHouse(this Address address)
		{
			string adr = address.FormattedAddress.ToLower();
			Assert.True(
				adr.Contains("The White House") ||
				adr.Contains("1600 pennsylvania ave nw") ||
				adr.Contains("1600 pennsylvania avenue northwest") ||
				adr.Contains("1600 pennsylvania avenue nw") ||
				adr.Contains("1600 pennsylvania ave northwest")
			);
			AssertWhiteHouseArea(address);
		}

		public static void AssertWhiteHouseArea(this Address address)
		{
			string adr = address.FormattedAddress.ToLower();
			Assert.True(
				adr.Contains("washington") &&
				(adr.Contains("dc") || adr.Contains("district of columbia"))
			);

			//just hoping that each geocoder implementation gets it somewhere near the vicinity
			double lat = Math.Round(address.Coordinates.Latitude, 2);
			Assert.Equal(38.90, lat);

			double lng = Math.Round(address.Coordinates.Longitude, 2);
			Assert.Equal(-77.04, lng);
		}

		public static void AssertCanadianPrimeMinister(this Address address)
		{
			string adr = address.FormattedAddress.ToLower();
			Assert.True(adr.Contains("24 sussex"));
			Assert.True(adr.Contains(" ottawa"));
			Assert.True(adr.Contains(" on"));
			Assert.True(adr.Contains("k1m"));
		}
	}
}