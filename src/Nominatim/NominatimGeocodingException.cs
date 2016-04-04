using System;

namespace Geocoding.Nominatim
{
    internal class NominatimGeocodingException : Exception
    {
        public NominatimGeocodingException()
        {
        }

        public NominatimGeocodingException(string message) : base(message)
        {
        }

        public NominatimGeocodingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}