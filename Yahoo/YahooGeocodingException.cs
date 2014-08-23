using System;

namespace Geocoding.Yahoo
{
	public class YahooGeocodingException : Exception
	{
		const string defaultMessage = "There was an error processing the geocoding request. See ErrorCode or InnerException for more information.";

		public YahooError ErrorCode { get; private set; }

		public YahooGeocodingException(YahooError errorCode)
			: base(defaultMessage)
		{
			this.ErrorCode = errorCode;
		}

		public YahooGeocodingException(Exception innerException)
			: base(defaultMessage, innerException)
		{
			this.ErrorCode = YahooError.UnknownError;
		}
	}
}