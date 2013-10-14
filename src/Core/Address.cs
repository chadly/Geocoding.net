using System;
using System.Text;

namespace GeoCoding
{
	public abstract class Address
	{
		readonly string formattedAddress;
		readonly Location coordinates;
		readonly string provider;

		public string FormattedAddress
		{
			get { return formattedAddress ?? ""; }
		}

		public Location Coordinates
		{
			get { return coordinates; }
		}

		public string Provider
		{
			get { return provider ?? ""; }
		}

		public Address(string formattedAddress, Location coordinates, string provider)
		{
			formattedAddress = (formattedAddress ?? "").Trim();

			if (String.IsNullOrEmpty(formattedAddress))
				throw new ArgumentNullException("formattedAddress");

			if (coordinates == null)
				throw new ArgumentNullException("coordinates");

			if (provider == null)
				throw new ArgumentNullException("provider");

			this.formattedAddress = formattedAddress;
			this.coordinates = coordinates;
			this.provider = provider;
		}

		public virtual Distance DistanceBetween(Address address)
		{
			return this.Coordinates.DistanceBetween(address.Coordinates);
		}

		public virtual Distance DistanceBetween(Address address, DistanceUnits units)
		{
			return this.Coordinates.DistanceBetween(address.Coordinates, units);
		}

		public override string ToString()
		{
			return FormattedAddress;
		}
	}
}