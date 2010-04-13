using System;

namespace GeoCoding
{
	public class Location
	{
		private readonly double latitude;
		private readonly double longitude;

		public double Latitude
		{
			get { return latitude; }
		}

		public double Longitude
		{
			get { return longitude; }
		}

		public Location(double latitude, double longitude)
		{
			if (longitude <= -180 || longitude > 180)
				throw new ArgumentOutOfRangeException("longitude", longitude, "Value must be between -180 and 180 (inclusive).");

			if (latitude < -90 || latitude > 90)
				throw new ArgumentOutOfRangeException("latitude", latitude, "Value must be between -90(inclusive) and 90(inclusive).");

			if (double.IsNaN(longitude))
				throw new ArgumentException("Longitude must be a valid number.", "longitude");

			if (double.IsNaN(latitude))
				throw new ArgumentException("Latitude must be a valid number.", "latitude");

			this.latitude = latitude;
			this.longitude = longitude;
		}

		private double ToRadian(double val)
		{
			return (Math.PI / 180.0) * val;
		}

		public Distance DistanceBetween(Location location)
		{
			return DistanceBetween(location, DistanceUnits.Miles);
		}

		public Distance DistanceBetween(Location location, DistanceUnits units)
		{
			double earthRadius = (units == DistanceUnits.Miles) ? Distance.EarthRadiusInMiles : Distance.EarthRadiusInKilometers;

			double latRadian = ToRadian(location.Latitude - this.Latitude);
			double longRadian = ToRadian(location.Longitude - this.Longitude);

			double a = Math.Pow(Math.Sin(latRadian / 2.0), 2) +
				Math.Cos(ToRadian(this.Latitude)) *
				Math.Cos(ToRadian(location.Latitude)) *
				Math.Pow(Math.Sin(longRadian / 2.0), 2);

			double c = 2.0 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

			double distance = earthRadius * c;
			return new Distance(distance, units);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Location);
		}

		public bool Equals(Location coor)
		{
			if (coor == null)
				return false;

			return (this.Latitude == coor.Latitude && this.Longitude == coor.Longitude);
		}

		public override int GetHashCode()
		{
			return Latitude.GetHashCode() ^ Latitude.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{0}, {1}", latitude, longitude);
		}
	}
}