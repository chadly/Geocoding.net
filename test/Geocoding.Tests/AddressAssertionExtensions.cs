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
				adr.Contains("1600") && (
				adr.Contains("pennsylvania ave nw") ||
				adr.Contains("pennsylvania avenue northwest") ||
				adr.Contains("pennsylvania avenue nw") ||
				adr.Contains("pennsylvania ave northwest"))
			);
			AssertWhiteHouseArea(address);
		}

		public static void AssertWhiteHouseArea(this Address address)
		{
			string adr = address.FormattedAddress.ToLower();
			Assert.True(
				adr.Contains("washington") &&
				(adr.Contains("dc") || adr.Contains("d.c.") || adr.Contains("district of columbia"))
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
			Assert.True(adr.Contains(" ontario"));
			//Assert.True(adr.Contains("k1m"));
		}

		public static void AssertReichstag(this Address address)
		{
			string adr = address.FormattedAddress.ToLower();
			Assert.True(adr.Contains("platz der republik"));
			Assert.True(adr.Contains("berlin"));
			Assert.True(adr.Contains("10557"));
			Assert.True(adr.Contains("deutschland") || adr.Contains("germany"));
		}
	}
}