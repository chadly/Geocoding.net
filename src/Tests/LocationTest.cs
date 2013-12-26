using Xunit;

namespace Geocoding.Tests
{
	public class LocationTest
	{
		[Fact]
		public void CanCreate()
		{
			const double lat = 85.6789;
			const double lon = 92.4517;

			Location loc = new Location(lat, lon);

			Assert.Equal(lat, loc.Latitude);
			Assert.Equal(lon, loc.Longitude);
		}

		[Fact]
		public void CanCompareForEquality()
		{
			Location loc1 = new Location(85.6789, 92.4517);
			Location loc2 = new Location(85.6789, 92.4517);

			Assert.True(loc1.Equals(loc2));
			Assert.Equal(loc1.GetHashCode(), loc2.GetHashCode());
		}

		[Fact]
		public void CanCalculateHaversineDistanceBetweenTwoAddresses()
		{
			Location loc1 = new Location(0, 0);
			Location loc2 = new Location(40, 20);

			Distance distance1 = loc1.DistanceBetween(loc2);
			Distance distance2 = loc2.DistanceBetween(loc1);

			Assert.Equal(distance1, distance2);
		}
	}
}