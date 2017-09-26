using System;

namespace Geocoding
{
	/// <summary>
	/// Most basic and generic form of address.
	/// Just the full address string and a lat/long
	/// </summary>
	public abstract class Address
	{
		string formattedAddress = string.Empty;
		Location coordinates;
		string provider = string.Empty;

		public Address(string formattedAddress, Location coordinates, string provider)
		{
			FormattedAddress = formattedAddress;
			Coordinates = coordinates;
			Provider = provider;
		}

		public virtual string FormattedAddress
		{
			get { return formattedAddress; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentException("FormattedAddress is null or blank");

				formattedAddress = value.Trim();
			}
		}

		public virtual Location Coordinates
		{
			get { return coordinates; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("Coordinates");

				coordinates = value;
			}
		}

		public virtual string Provider
		{
			get { return provider; }
			protected set
			{
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentException("Provider can not be null or blank");

				provider = value;
			}
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