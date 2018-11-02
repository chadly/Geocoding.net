using System;
using Geocoding.Core;

namespace Geocoding.Microsoft
{
	public class BingGeocodingException : GeocodingException
	{
		const string defaultMessage = "There was an error processing the geocoding request. See InnerException for more information.";

		public BingGeocodingException(Exception innerException)
			: base(defaultMessage, innerException) { }
	}
}