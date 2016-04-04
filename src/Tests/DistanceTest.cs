using Xunit;
using Xunit.Extensions;

namespace Geocoding.Tests
{
	public class DistanceTest
	{
		[Fact]
		public void CanCreate()
		{
			Distance distance = new Distance(5.7, DistanceUnits.Miles);

			Assert.Equal(5.7, distance.Value);
			Assert.Equal(DistanceUnits.Miles, distance.Units);
		}

		[Fact]
		public void CanRoundValueToEightDecimalPlaces()
		{
			Distance distance = new Distance(0.123456789101112131415, DistanceUnits.Miles);
			Assert.Equal(0.12345679, distance.Value);
		}

		[Fact]
		public void CanCompareForEquality()
		{
			Distance distance1 = new Distance(5, DistanceUnits.Miles);
			Distance distance2 = new Distance(5, DistanceUnits.Miles);

			Assert.True(distance1.Equals(distance2));
			Assert.Equal(distance1.GetHashCode(), distance2.GetHashCode());
		}

		[Theory]
		[InlineData(1, 1.609344)]
		[InlineData(0.621371192, 1)]
		[InlineData(1, 1)]
		[InlineData(0, 0)]
		[InlineData(5, 6)]
		public void CanCompareForEqualityWithNormalizedUnits(double miles, double kilometers)
		{
			Distance mileDistance = Distance.FromMiles(miles);
			Distance kilometerDistance = Distance.FromKilometers(kilometers);

			bool expected = mileDistance.Equals(kilometerDistance.ToMiles());
			bool actual = mileDistance.Equals(kilometerDistance, true);

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(-5, -8.04672)]
		[InlineData(0, 0)]
		[InlineData(1, 1.609344)]
		[InlineData(5, 8.04672)]
		[InlineData(10, 16.09344001)]
		public void CanConvertFromMilesToKilometers(double miles, double expectedKilometers)
		{
			Distance mileDistance = Distance.FromMiles(miles);
			Distance kilometerDistance = mileDistance.ToKilometers();

			Assert.Equal(expectedKilometers, kilometerDistance.Value);
			Assert.Equal(DistanceUnits.Kilometers, kilometerDistance.Units);
		}

		[Theory]
		[InlineData(-5, -3.10685596)]
		[InlineData(0, 0)]
		[InlineData(1, 0.62137119)]
		[InlineData(5, 3.10685596)]
		[InlineData(10, 6.21371192)]
		public void CanConvertFromKilometersToMiles(double kilometers, double expectedMiles)
		{
			Distance kilometerDistance = Distance.FromKilometers(kilometers);
			Distance mileDistance = kilometerDistance.ToMiles();

			Assert.Equal(expectedMiles, mileDistance.Value);
			Assert.Equal(DistanceUnits.Miles, mileDistance.Units);
		}

		#region Operator Tests

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanMultiply(double value, double multiplier)
		{
			Distance distance1 = Distance.FromMiles(value);

			Distance expected = Distance.FromMiles(value * multiplier);
			Distance actual = distance1 * multiplier;

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanAdd(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromMiles(right);

			Distance expected = Distance.FromMiles(left + right);
			Distance actual = distance1 + distance2;

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanSubtract(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromMiles(right);

			Distance expected = Distance.FromMiles(left - right);
			Distance actual = distance1 - distance2;

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(5, -5)]
		[InlineData(3, 3)]
		[InlineData(3.8, 3.8)]
		public void CanCompareWithEqualSign(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromMiles(right);

			bool expectedEqual = (left == right);
			Assert.Equal(expectedEqual, distance1 == distance2);
			Assert.Equal(!expectedEqual, distance1 != distance2);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanCompareLessThan(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromMiles(right);

			bool expected = left < right;
			Assert.Equal(expected, distance1 < distance2);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanCompareLessThanEqualTo(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromMiles(right);

			bool expected = left <= right;
			Assert.Equal(expected, distance1 <= distance2);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanCompareGreaterThan(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromMiles(right);

			bool expected = left > right;
			Assert.Equal(expected, distance1 > distance2);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanCompareGreaterThanEqualTo(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromMiles(right);

			bool expected = left >= right;
			Assert.Equal(expected, distance1 >= distance2);
		}

		[Fact]
		public void CanImplicitlyConvertToDouble()
		{
			Distance distance = Distance.FromMiles(56);
			double d = distance;
			Assert.Equal(d, distance.Value);
		}

		#endregion

		#region Operator Conversion Tests

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanAddWithDifferentUnits(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromKilometers(right);

			Distance expected = distance1 + distance2.ToMiles();
			Distance actual = distance1 + distance2;

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanSubtractWithDifferentUnits(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromKilometers(right);

			Distance expected = distance1 - distance2.ToMiles();
			Distance actual = distance1 - distance2;

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanCompareLessThanWithDifferentUnits(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromKilometers(right);

			bool expected = distance1 < distance2.ToMiles();
			Assert.Equal(expected, distance1 < distance2);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanCompareLessThanEqualToWithDifferentUnits(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromKilometers(right);

			bool expected = distance1 <= distance2.ToMiles();
			Assert.Equal(expected, distance1 <= distance2);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanCompareGreaterThanWithDifferentUnits(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromKilometers(right);

			bool expected = distance1 > distance2.ToMiles();
			Assert.Equal(expected, distance1 > distance2);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0.45359237)]
		[InlineData(9, 5)]
		[InlineData(5, -5)]
		[InlineData(3, 0)]
		public void CanCompareGreaterThanEqualToWithDifferentUnits(double left, double right)
		{
			Distance distance1 = Distance.FromMiles(left);
			Distance distance2 = Distance.FromKilometers(right);

			bool expected = distance1 >= distance2.ToMiles();
			Assert.Equal(expected, distance1 >= distance2);
		}

		#endregion
	}
}
