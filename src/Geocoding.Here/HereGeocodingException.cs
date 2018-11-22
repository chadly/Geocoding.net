using System;
using Geocoding.Core;

namespace Geocoding.Here
{
	public class HereGeocodingException : GeocodingException
	{
		const string defaultMessage = "There was an error processing the geocoding request. See InnerException for more information.";

		public HereGeocodingException(Exception innerException)
			: base(defaultMessage, innerException)
		{
		}
	}
}
