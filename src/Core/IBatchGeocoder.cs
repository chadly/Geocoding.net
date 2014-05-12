using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geocoding
{
	public interface IBatchGeocoder
	{
		ICollection<ResultItem> Geocode(IEnumerable<string> addresses);
		ICollection<ResultItem> ReverseGeocode(IEnumerable<Location> locations);
	}
}
