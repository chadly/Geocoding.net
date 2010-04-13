using System;
using System.Text;

namespace GeoCoding
{
	public class Address
	{
		private readonly string street;
		private readonly string city;
		private readonly string state;
		private readonly string postalCode;
		private readonly string country;
		private readonly Location coordinates;
		private readonly AddressAccuracy accuracy;

		public string Street
		{
			get { return street ?? ""; }
		}

		public string City
		{
			get { return city ?? ""; }
		}

		public string State
		{
			get { return state ?? ""; }
		}

		public string PostalCode
		{
			get { return postalCode ?? ""; }
		}

		public string Country
		{
			get { return country ?? ""; }
		}

		public Location Coordinates
		{
			get { return coordinates; }
		}

		public AddressAccuracy Accuracy
		{
			get { return accuracy; }
		}

		public Address(string street, string city, string state, string postalCode, string country, Location coordinates, AddressAccuracy accuracy)
		{
			this.street = street;
			this.city = city;
			this.state = state;
			this.postalCode = postalCode;
			this.country = country;
			this.coordinates = coordinates;
			this.accuracy = accuracy;
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
			StringBuilder address = new StringBuilder();

			bool hasStuff = false;

			if (!String.IsNullOrEmpty(street))
			{
				address.Append(street);
				hasStuff = true;
			}

			if (!String.IsNullOrEmpty(city))
			{
				if (hasStuff)
					address.Append(", ");
				address.Append(city);
				hasStuff = true;
			}

			if (!String.IsNullOrEmpty(state))
			{
				if (hasStuff)
					address.Append(", ");
				address.Append(state);
				hasStuff = true;
			}

			if (!String.IsNullOrEmpty(postalCode))
			{
				if (hasStuff)
					address.Append(" ");
				address.Append(postalCode);
				hasStuff = true;
			}

			if (!String.IsNullOrEmpty(country))
			{
				if (hasStuff)
					address.Append(", ");
				address.Append(country);
			}

			return address.ToString();
		}
	}
}