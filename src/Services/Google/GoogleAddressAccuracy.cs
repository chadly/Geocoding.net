using System;

namespace GeoCoding.Services.Google
{
    public enum GoogleAddressAccuracy
    {
        UnknownLocation,
        CountryLevel,
        RegionLevel,
        SubRegionLevel,
        TownLevel,
        ZipCodeLevel,
        StreetLevel,
        IntersectionLevel,
        AddressLevel
    }
}
