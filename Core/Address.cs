using System;
using System.Text;

namespace GeoCoding
{
    public class Address
    {
        private string street;
        private string city;
        private string state;
        private string postalCode;
        private string country;
        private Location coordinates;
        private AddressAccuracy accuracy;

        public string Street
        {
            get { return street ?? ""; }
            set { street = value; }
        }

        public string City
        {
            get { return city ?? ""; }
            set { city = value; }
        }

        public string State
        {
            get { return state ?? ""; }
            set { state = value; }
        }

        public string PostalCode
        {
            get { return postalCode ?? ""; }
            set { postalCode = value; }
        }

        public string Country
        {
            get { return country ?? ""; }
            set { country = value; }
        }

        public Location Coordinates
        {
            get { return coordinates; }
        }

        public AddressAccuracy Accuracy
        {
            get { return accuracy; }
        }

        public Address()
        {
            this.coordinates = Location.Empty;
            this.accuracy = AddressAccuracy.Unknown;
        }

        public virtual Distance DistanceBetween(Address address)
        {
            return this.Coordinates.DistanceBetween(address.Coordinates);
        }

        public virtual Distance DistanceBetween(Address address, DistanceUnits units)
        {
            return this.Coordinates.DistanceBetween(address.Coordinates, units);
        }

        public void ChangeLocation(Location coordinates, AddressAccuracy accuracy)
        {
            this.coordinates = coordinates;
            this.accuracy = accuracy;
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
