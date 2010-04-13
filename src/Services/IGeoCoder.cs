using System;

namespace GeoCoding
{
    public interface IGeoCoder
    {
        Address[] GeoCode(string address);
        Address[] GeoCode(string street, string city, string state, string postalCode, string country);
    }
}
