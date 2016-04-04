using System;

namespace Geocoding.GeoNames
{
    public class GeoNamesGeocodingException : Exception
    {
        public GeoNamesGeocodingException()
        {

        }

        public GeoNamesGeocodingException(string message) : base(message)
        {

        }

        public GeoNamesGeocodingException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}