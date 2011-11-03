using System;

namespace GeoCoding.Google
{
	public class GoogleGeoCodingException : Exception
	{
		const string defaultMessage = "There was an error processing the geocoding request. See Status for more information.";

		public GoogleStatus Status { get; private set; }

		public GoogleGeoCodingException(GoogleStatus status)
			: base(defaultMessage)
		{
			this.Status = status;
		}

		public GoogleGeoCodingException(Exception innerException)
			: base(defaultMessage, innerException)
		{
			this.Status = GoogleStatus.Error;
		}
	}
}