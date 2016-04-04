using System.Collections.Generic;

namespace Geocoding
{
    public interface IBatchGeocoder
    {
        IEnumerable<ResultItem> Geocode(IEnumerable<string> addresses);

        IEnumerable<ResultItem> ReverseGeocode(IEnumerable<Location> locations);
    }
}