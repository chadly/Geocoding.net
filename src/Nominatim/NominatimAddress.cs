namespace Geocoding.Nominatim
{
    internal class NominatimAddress : Address
    {
        public NominatimAddress(string formattedAddress, Location coordinates) : base(formattedAddress, coordinates, "Nominatim")
        {
        }

        public string Suburb { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Hamlet { get; set; }
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }
        public string Road { get; set; }
        public string StateDistrict { get; set; }
        public string State { get; set; }
    }
}