using System;

namespace GeoCoding.Services
{
    public interface IGeoCoder
    {
        Address[] GeoCode(string address);
        Address[] GeoCode(string street, string city, string state, string postalCode, Country country);
        Address[] Validate(Address address);
    }
}
