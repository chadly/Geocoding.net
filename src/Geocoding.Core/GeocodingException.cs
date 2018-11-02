using System;

namespace Geocoding.Core
{
	public class GeocodingException : Exception
	{
		public GeocodingException(string message, Exception innerException = null)
			: base(message, innerException)
		{
		}
	}
}
