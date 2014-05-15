using System;

namespace Geocoding
{
	public class Bounds
	{
		readonly Location southWest;
		readonly Location northEast;

		public Location SouthWest
		{
			get { return southWest; }
		}

		public Location NorthEast
		{
			get { return northEast; }
		}

		public Bounds(double southWestLatitude, double southWestLongitude, double northEastLatitude, double northEastLongitude)
			: this(new Location(southWestLatitude, southWestLongitude), new Location(northEastLatitude, northEastLongitude)) { }

		public Bounds(Location southWest, Location northEast)
		{
			if (southWest == null)
				throw new ArgumentNullException("southWest");

			if (northEast == null)
				throw new ArgumentNullException("northEast");

			if (southWest.Latitude > northEast.Latitude)
				throw new ArgumentException("southWest latitude cannot be greater than northEast latitude");

			this.southWest = southWest;
			this.northEast = northEast;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Bounds);
		}

		public bool Equals(Bounds bounds)
		{
			if (bounds == null)
				return false;

			return (this.SouthWest.Equals(bounds.SouthWest) && this.NorthEast.Equals(bounds.NorthEast));
		}

		public override int GetHashCode()
		{
			return SouthWest.GetHashCode() ^ NorthEast.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0} | {1}", southWest, northEast);
		}
	}
}
