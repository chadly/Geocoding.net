using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
