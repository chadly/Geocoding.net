using System;
using Geocoding.Core;

namespace Geocoding.Here
{
	public class HereGeocodingException : GeocodingException
	{
		const string defaultMessage = "There was an error processing the geocoding request. See InnerException for more information.";

		public string ErrorType { get; }

		public string ErrorSubtype { get; }

		public HereGeocodingException(Exception innerException)
			: base(defaultMessage, innerException)
		{
		}

		public HereGeocodingException(string message, string errorType, string errorSubtype)
			: base(message)
		{
			ErrorType = errorType;
			ErrorSubtype = errorSubtype;
		}
	}
}
