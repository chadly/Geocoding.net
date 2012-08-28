using System;
using System.Collections.Generic;

namespace GeoCoding
{
	public interface IGeoCoder
	{
		IEnumerable<Address> GeoCode(string address);
		IEnumerable<Address> GeoCode(string street, string city, string state, string postalCode, string country);

		IEnumerable<Address> ReverseGeocode(Location location);
		IEnumerable<Address> ReverseGeocode(double latitude, double longitude);
	}
}