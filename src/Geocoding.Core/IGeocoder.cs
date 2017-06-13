using System.Collections.Generic;
using System.Threading.Tasks;

namespace Geocoding
{
	public interface IGeocoder
	{
		Task<IEnumerable<Address>> GeocodeAsync(string address);
		Task<IEnumerable<Address>> GeocodeAsync(string street, string city, string state, string postalCode, string country);

		Task<IEnumerable<Address>> ReverseGeocodeAsync(Location location);
		Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude);
	}
}