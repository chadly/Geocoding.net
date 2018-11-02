using System;
using Geocoding.Core;

namespace Geocoding.Google
{
	public class GoogleGeocodingException : GeocodingException
	{
		const string defaultMessage = "There was an error processing the geocoding request. See Status or InnerException for more information.";

		public GoogleStatus Status { get; private set; }

		public GoogleGeocodingException(GoogleStatus status)
			: base(defaultMessage)
		{
			this.Status = status;
		}

		public GoogleGeocodingException(Exception innerException)
			: base(defaultMessage, innerException)
		{
			this.Status = GoogleStatus.Error;
		}
	}
}