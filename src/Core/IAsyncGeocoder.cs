using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Geocoding
{
    public interface IAsyncGeocoder
    {
        Task<IEnumerable<Address>> GeocodeAsync(string address);

        Task<IEnumerable<Address>> GeocodeAsync(string address, CancellationToken cancellationToken);

        Task<IEnumerable<Address>> GeocodeAsync(string street, string city, string state, string postalCode, string country);

        Task<IEnumerable<Address>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken);

        Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude);

        Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken);
    }
}