using System;

namespace GeoCoding
{
    public struct Address
    {
        #region Properties

        public static readonly Address Empty = new Address();

        private readonly string _street;
        private readonly string _city;
        private readonly string _state;
        private readonly string _postalCode;
        private readonly string _country;
        private readonly Location _coordinates;
        private readonly AddressAccuracy _accuracy;

        public string Street
        {
            get { return _street ?? ""; }
        }

        public string City
        {
            get { return _city ?? ""; }
        }

        public string State
        {
            get { return _state ?? ""; }
        }

        public string PostalCode
        {
            get { return _postalCode ?? ""; }
        }

        public string Country
        {
            get { return _country ?? ""; }
        }

        public Location Coordinates
        {
            get { return _coordinates; }
        }

        public AddressAccuracy Accuracy
        {
            get { return _accuracy; }
        }

        #endregion

        #region Constructors

        public Address(string street, string city, string state, string postalCode, string country)
            : this(street, city, state, postalCode, country, Location.Empty, AddressAccuracy.Unknown) { }

        public Address(string street, string city, string state, string postalCode, string country, Location coordinates, AddressAccuracy accuracy)
        {
			_street = !String.IsNullOrEmpty(street) ? street : null;
			_city = !String.IsNullOrEmpty(city) ? city : null;
			_state = !String.IsNullOrEmpty(state) ? state : null;
			_postalCode = !String.IsNullOrEmpty(postalCode) ? postalCode : null;
			_country = !String.IsNullOrEmpty(country) ? country : null;
            _coordinates = coordinates;
            _accuracy = accuracy;
        }

        #endregion

        public Distance DistanceBetween(Address address)
        {
            return this.Coordinates.DistanceBetween(address.Coordinates);
        }

        public Distance DistanceBetween(Address address, DistanceUnits units)
        {
            return this.Coordinates.DistanceBetween(address.Coordinates, units);
        }

        public override string ToString()
        {
            return String.Format("{0} {1}, {2} {3}, {4}", _street, _city, _state, _postalCode, _country);
        }
    }
}
