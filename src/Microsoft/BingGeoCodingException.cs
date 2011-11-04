using System;

namespace GeoCoding.Microsoft
{
	public class BingGeoCodingException : Exception
	{
		const string defaultMessage = "There was an error processing the geocoding request. See InnerException for more information.";

		public BingGeoCodingException(Exception innerException)
			: base(defaultMessage, innerException) { }
	}
}