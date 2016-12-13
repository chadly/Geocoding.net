using System.Collections.Generic;
using System.Threading.Tasks;

namespace Geocoding
{
	public interface IGeocoder
	{
        Task<IEnumerable<Address>>  Geocode(string address);
        Task<IEnumerable<Address>> Geocode(string street, string city, string state, string postalCode, string country);

        Task<IEnumerable<Address>> ReverseGeocode(Location location);
        Task<IEnumerable<Address>> ReverseGeocode(double latitude, double longitude);
	}
}