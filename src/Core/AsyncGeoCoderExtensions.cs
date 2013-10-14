using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GeoCoding
{
    public static class AsyncGeoCoderExtensions
    {
        public static Task<IEnumerable<Address>> ReverseGeocodeAsync(this IAsyncGeoCoder geoCoder, Location location)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            return geoCoder.ReverseGeocodeAsync(location.Latitude, location.Longitude);
        }

        public static Task<IEnumerable<Address>> ReverseGeocodeAsync(this IAsyncGeoCoder geoCoder, Location location, CancellationToken cancellationToken)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            return geoCoder.ReverseGeocodeAsync(location.Latitude, location.Longitude, cancellationToken);
        }
    }
}