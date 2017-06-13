namespace Geocoding.Yahoo
{
	public class YahooAddress : Address
	{
		readonly string name, house, street, unit, unitType, neighborhood, city, county, countyCode, state, stateCode, postalCode, country, countryCode;
		readonly int quality;

		public string Name
		{
			get { return name ?? ""; }
		}

		public string House
		{
			get { return house ?? ""; }
		}

		public string Street
		{
			get { return street ?? ""; }
		}

		public string Unit
		{
			get { return unit ?? ""; }
		}

		public string UnitType
		{
			get { return unitType ?? ""; }
		}

		public string Neighborhood
		{
			get { return neighborhood ?? ""; }
		}

		public string City
		{
			get { return city ?? ""; }
		}

		public string County
		{
			get { return county ?? ""; }
		}

		public string CountyCode
		{
			get { return countyCode ?? ""; }
		}

		public string State
		{
			get { return state ?? ""; }
		}

		public string StateCode
		{
			get { return stateCode ?? ""; }
		}

		public string PostalCode
		{
			get { return postalCode ?? ""; }
		}

		public string Country
		{
			get { return country ?? ""; }
		}

		public string CountryCode
		{
			get { return countryCode ?? ""; }
		}

		public int Quality
		{
			get { return quality; }
		}

		/// <remarks>
		/// http://developer.yahoo.com/geo/placefinder/guide/responses.html#address-quality
		/// </remarks>
		public string QualityDescription
		{
			get
			{
				switch (Quality)
				{
					case 99: return "Coordinate";
					case 90: return "POI";
					case 87: return "Address match with street match";
					case 86: return "Address mismatch with street match";
					case 85: return "Address match with street mismatch";
					case 84: return "Address mismatch with street mismatch";
					case 82: return "Intersection with street match";
					case 80: return "Intersection with street mismatch";

					case 75: return "Postal unit/segment (Zip+4 in US)";
					case 74: return "Postal unit/segment, street ignored (Zip+4 in US)";
					case 72: return "Street match";
					case 71: return "Street match, address ignored";
					case 70: return "Street mismatch";

					case 64: return "Postal zone/sector, street ignored (Zip+2 in US)";
					case 63: return "AOI";
					case 62: return "Airport";
					case 60: return "Postal district (Zip Code in US)";
					case 59: return "Postal district, street ignored (Zip Code in US)";
					case 50: return "Level4 (Neighborhood)";
					case 49: return "Level4, street ignored (Neighborhood)";
					case 40: return "Level3 (City/Town/Locality)";
					case 39: return "Level3, level4 ignored (City/Town/Locality)";
					case 30: return "Level2 (County)";
					case 29: return "Level2, level3 ignored (County)";
					case 20: return "Level1 (State/Province)";
					case 19: return "Level1, level2 ignored (State/Province)";
					case 10: return "Level0 (Country)";
					case 9: return "Level0, level1 ignored (Country)";
					case 0: return "Not an address";

					default: return "Unknown";
				}
			}
		}

		public YahooAddress(string formattedAddress, Location coordinates, string name, string house, string street,
			string unit, string unitType, string neighborhood, string city, string county, string countyCode, string state,
			string stateCode, string postalCode, string country, string countryCode, int quality)
			: base(formattedAddress, coordinates, "Yahoo")
		{
			this.name = name;
			this.house = house;
			this.street = street;
			this.unit = unit;
			this.unitType = unitType;
			this.neighborhood = neighborhood;
			this.city = city;
			this.county = county;
			this.countyCode = countyCode;
			this.state = state;
			this.stateCode = stateCode;
			this.postalCode = postalCode;
			this.country = country;
			this.countryCode = countryCode;
			this.quality = quality;
		}
	}
}