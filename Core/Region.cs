using System;

namespace GeoCoding
{
	public struct Region
	{
		public static readonly Region Empty = new Region();

		private readonly string _postalCode;
		private readonly string _city;
		private readonly string _state;
		private readonly string _country;
		private readonly Location _coordinates;
		private readonly RegionLevel _level;

		public string PostalCode
		{
			get { return _postalCode ?? ""; }
		}

		public string City
		{
			get { return _city ?? ""; }
		}

		public string State
		{
			get { return _state ?? ""; }
		}

		public string Country
		{
			get { return _country ?? ""; }
		}

		public Location Coordinates
		{
			get { return _coordinates; }
		}

		public RegionLevel Level
		{
			get { return _level; }
		}

		public Region(string country, string state, string city,
			string postalCode, Location coordinates, RegionLevel level)
		{
			_country = !String.IsNullOrEmpty(country) ? country : null;
			_state = !String.IsNullOrEmpty(state) ? state : null;
			_city = !String.IsNullOrEmpty(city) ? city : null;
			_postalCode = !String.IsNullOrEmpty(postalCode) ? postalCode : null;
			_coordinates = coordinates;
			_level = level;
		}

		public bool Contains(Address address)
		{
			switch (Level)
			{
				case RegionLevel.CountryLevel:
					return address.Country == this.Country;
				case RegionLevel.StateLevel:
					return address.Country == this.Country
						&& address.State == this.State;
				case RegionLevel.CityLevel:
					return address.Country == this.Country
						&& address.State == this.State
						&& address.City == this.City;
				case RegionLevel.PostalCodeLevel:
					return address.Country == this.Country
						&& address.State == this.State
						&& address.City == this.City
						&& address.PostalCode == this.PostalCode;
				default:
					return false;
			}
		}

		private static RegionLevel LevelFromAccuracy(AddressAccuracy accuracy)
		{
			switch (accuracy)
			{
				case AddressAccuracy.CountryLevel:
					return RegionLevel.CountryLevel;
				case AddressAccuracy.StateLevel:
					return RegionLevel.StateLevel;
				case AddressAccuracy.CityLevel:
					return RegionLevel.CityLevel;
				case AddressAccuracy.PostalCodeLevel:
					return RegionLevel.PostalCodeLevel;
				case AddressAccuracy.StreetLevel:
					return RegionLevel.PostalCodeLevel;
				case AddressAccuracy.AddressLevel:
					return RegionLevel.PostalCodeLevel;
				default:
					return RegionLevel.Unknown;
			}
		}

		public static implicit operator Region(Address address)
		{
			return new Region(
				address.Country,
				address.State,
				address.City,
				address.PostalCode,
				address.Coordinates,
				LevelFromAccuracy(address.Accuracy)
			);
		}

		public bool Equals(Region region)
		{
			return Equals((object)region);
		}

		public override string ToString()
		{
			if (Equals(Region.Empty))
				return "Region.Empty";

			switch (Level)
			{
				case RegionLevel.CountryLevel:
					return String.Format("{0}", Country);
				case RegionLevel.StateLevel:
					return String.Format("{0}, {1}", State, Country);
				case RegionLevel.CityLevel:
					return String.Format("{0}, {1}, {2}", City, State, Country);
				case RegionLevel.PostalCodeLevel:
				default:
					return String.Format("{0}, {1} {2}, {3}", City, State, PostalCode, Country);
			}
		}
	}
}
