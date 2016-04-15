using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Geocoding
{
	public static class AsyncGeocoderExtensions
	{
		public static Task<IEnumerable<Address>> ReverseGeocodeAsync(this IAsyncGeocoder geoCoder, Location location)
		{
			if (location == null)
				throw new ArgumentNullException("location");

			return geoCoder.ReverseGeocodeAsync(location.Latitude, location.Longitude);
		}

		public static Task<IEnumerable<Address>> ReverseGeocodeAsync(this IAsyncGeocoder geoCoder, Location location, CancellationToken cancellationToken)
		{
			if (location == null)
				throw new ArgumentNullException("location");

			return geoCoder.ReverseGeocodeAsync(location.Latitude, location.Longitude, cancellationToken);
		}
	}
}