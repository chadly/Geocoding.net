using System.Collections.Generic;

namespace Geocoding
{
	public interface IGeocoder
	{
		IEnumerable<Address> Geocode(string address);

		IEnumerable<Address> Geocode(string street, string city, string state, string postalCode, string country);

		IEnumerable<Address> ReverseGeocode(Location location);

		IEnumerable<Address> ReverseGeocode(double latitude, double longitude);
	}
}