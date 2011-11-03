using System;

namespace GeoCoding
{
	public class YahooAddress : Address
	{
		readonly string street;
		readonly string city;
		readonly string state;
		readonly string postalCode;
		readonly string country;
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

		public AddressAccuracy Accuracy
		{
			get { return accuracy; }
		}

		public YahooAddress(string street, string city, string state, string postalCode, string country, Location coordinates, AddressAccuracy accuracy, string formattedAddress)
			: base(formattedAddress, coordinates)
		{
			this.street = street;
			this.city = city;
			this.state = state;
			this.postalCode = postalCode;
			this.country = country;
			this.accuracy = accuracy;
		}
	}
}