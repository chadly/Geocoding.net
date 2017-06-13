using System.Collections.Generic;
using System.Threading.Tasks;

namespace Geocoding
{
	public interface IBatchGeocoder
	{
		Task<IEnumerable<ResultItem>> GeocodeAsync(IEnumerable<string> addresses);
		Task<IEnumerable<ResultItem>> ReverseGeocodeAsync(IEnumerable<Location> locations);
	}
}
