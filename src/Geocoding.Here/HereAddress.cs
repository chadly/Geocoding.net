using System;

namespace Geocoding.Here
{
	public class HereAddress : Address
	{
		readonly string street, houseNumber, city, state, country, postalCode, resultType;

		public string AddressLine
		{
			get { return street ?? ""; }
		}

		public string AdminDistrict
		{
			get { return houseNumber ?? ""; }
		}

		public string AdminDistrict2
		{
			get { return city ?? ""; }
		}

		public string CountryRegion
		{
			get { return state ?? ""; }
		}

		public string Neighborhood
		{
			get { return country ?? ""; }
		}

		public string PostalCode
		{
			get { return postalCode ?? ""; }
		}

		public String ResultType
		{
			get { return resultType; }
		}

		public HereAddress(string formattedAddress, Location coordinates, string street, string houseNumber, string city,
			string state, string postalCode, string country, string resultType)
			: base(formattedAddress, coordinates, "HERE")
		{
			this.street = street;
			this.houseNumber = houseNumber;
			this.city = city;
			this.state = state;
			this.postalCode = postalCode;
			this.country = country;
			this.resultType = resultType;
		}
	}
}
