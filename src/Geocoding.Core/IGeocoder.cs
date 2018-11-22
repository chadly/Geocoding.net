using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Geocoding
{
	public interface IGeocoder
	{
		Task<IEnumerable<Address>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken));
		Task<IEnumerable<Address>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default(CancellationToken));

		Task<IEnumerable<Address>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default(CancellationToken));
		Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default(CancellationToken));
	}
}