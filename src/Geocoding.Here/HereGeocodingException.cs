using System;
using Geocoding.Core;

namespace Geocoding.Here
{
	public class HereGeocodingException : GeocodingException
	{
		const string defaultMessage = "There was an error processing the geocoding request. See InnerException for more information.";

		public int StatusCode { get; set; }
		public string Cause { get; set; }
		public string Action { get; set; }
		public string CorrelationId { get; set; }
		public string RequestId { get; set; }

		public HereGeocodingException(Exception innerException)
			: base(defaultMessage, innerException)
		{
		}

		public HereGeocodingException(string message, int statusCode, string cause, string action, string correlationId, string requestId)
			: base(message)
		{
			StatusCode = statusCode;
			Cause = cause;
			Action = action;
			CorrelationId = correlationId;
			RequestId = requestId;
		}
	}
}
