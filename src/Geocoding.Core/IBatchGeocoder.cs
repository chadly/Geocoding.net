using System.Collections.Generic;
using System.Threading.Tasks;

namespace Geocoding
{
	public interface IBatchGeocoder
	{
		Task<IEnumerable<ResultItem>> Geocode(IEnumerable<string> addresses);
        Task<IEnumerable<ResultItem>> ReverseGeocode(IEnumerable<Location> locations);
	}
}
