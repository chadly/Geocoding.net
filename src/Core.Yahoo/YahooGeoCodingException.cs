using System;

namespace GeoCoding.Yahoo
{
	public class YahooGeoCodingException : Exception
	{
		const string defaultMessage = "There was an error processing the geocoding request. See ErrorCode or InnerException for more information.";

		public YahooError ErrorCode { get; private set; }

		public YahooGeoCodingException(YahooError errorCode)
			: base(defaultMessage)
		{
			this.ErrorCode = errorCode;
		}

		public YahooGeoCodingException(Exception innerException)
			: base(defaultMessage, innerException)
		{
			this.ErrorCode = YahooError.UnknownError;
		}
	}
}