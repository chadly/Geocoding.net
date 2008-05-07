using System;

namespace GeoCoding
{
    public struct Location
    {
        public static readonly Location Empty = new Location();

        private readonly double _latitude;
        private readonly double _longitude;

        public double Latitude
        {
            get { return _latitude; }
        }

        public double Longitude
        {
            get { return _longitude; }
        }

        public Location(double latitude, double longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
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

        public override string ToString()
        {
            return String.Format("{0}, {1}", _latitude, _longitude);
        }
    }
}
