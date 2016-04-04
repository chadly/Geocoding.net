using System;

namespace Geocoding.Microsoft
{
    public class BingGeocodingException : Exception
    {
        private const string defaultMessage = "There was an error processing the geocoding request. See InnerException for more information.";

        public BingGeocodingException(Exception innerException)
            : base(defaultMessage, innerException)
        { }
    }
}