using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace GeoCoding
{
    public interface IAsyncGeoCoder
    {
        Task<IEnumerable<Address>> GeoCodeAsync(string address);
        Task<IEnumerable<Address>> GeoCodeAsync(string address, CancellationToken cancellationToken);
        Task<IEnumerable<Address>> GeoCodeAsync(string street, string city, string state, string postalCode, string country);
        Task<IEnumerable<Address>> GeoCodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken);

        Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude);
        Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken);
    }
}
