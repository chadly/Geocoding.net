using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Geocoding
{
	public interface IBatchGeocoder
	{
		Task<IEnumerable<ResultItem>> GeocodeAsync(IEnumerable<string> addresses, CancellationToken cancellationToken = default(CancellationToken));
		Task<IEnumerable<ResultItem>> ReverseGeocodeAsync(IEnumerable<Location> locations, CancellationToken cancellationToken = default(CancellationToken));
	}
}
