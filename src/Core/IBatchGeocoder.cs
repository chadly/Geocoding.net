using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geocoding
{
	public interface IBatchGeocoder
	{
		IEnumerable<ResultItem> Geocode(IEnumerable<string> addresses);
		IEnumerable<ResultItem> ReverseGeocode(IEnumerable<Location> locations);
	}
}
