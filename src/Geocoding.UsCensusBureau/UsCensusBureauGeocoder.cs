using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Geocoding.UsCensusBureau
{
	/// <summary>
	/// refs:
	/// - https://geocoding.geo.census.gov/
	/// - https://geocoding.geo.census.gov/geocoder/Geocoding_Services_API.pdf
	/// </summary>
	public class UsCensusBureauGeocoder : IGeocoder
    {
        private readonly int _benchmark;
	    private readonly string _format;
	    private readonly HttpClient _client;

        public UsCensusBureauGeocoder(int benchmark = 4, string format = "json")
        {
            _benchmark = benchmark;
	        _format = format;
	        _client = new HttpClient { BaseAddress = new Uri(UsCensusBureauConstants.BaseUrl) };
        }
        
        public async Task<IEnumerable<Address>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Build Query String
			var sb = new StringBuilder(UsCensusBureauConstants.OneLineAddressPath);
			sb.Append("address=").Append(WebUtility.UrlEncode(address))
				.Append("&benchmark=").Append(_benchmark)
				.Append("&format=").Append(_format);
            
            // Get Request
            var response = await _client.GetAsync(sb.ToString(), cancellationToken);
	        var content = await response.Content.ReadAsStringAsync();
            
            // Read Result
            return GetAddresses(content);
        }

        public async  Task<IEnumerable<Address>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default(CancellationToken))
        {
			// Build Query String
			var sb = new StringBuilder(UsCensusBureauConstants.AddressPath);
			sb.Append("street=").Append(WebUtility.UrlEncode(street))
				.Append("&city=").Append(WebUtility.UrlEncode(city))
				.Append("&state=").Append(WebUtility.UrlEncode(state))
				.Append("&zip=").Append(WebUtility.UrlEncode(postalCode))
				.Append("&benchmark=").Append(_benchmark)
				.Append("&format=").Append(_format);
            
	        // Get Request
	        var response = await _client.GetAsync(sb.ToString(), cancellationToken);
	        var content = await response.Content.ReadAsStringAsync();
            
	        // Read Result
	        return GetAddresses(content);
        }

        public Task<IEnumerable<Address>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotSupportedException();
        }

        public Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotSupportedException();
        }

        private static IEnumerable<UsCensusBureauAddress> GetAddresses(string response)
        {
	        var json = JObject.Parse(response);
	        
	        var errors = json[UsCensusBureauConstants.ErrorsKey];
	        if (errors != null)
		        return new UsCensusBureauAddress[] {};
	        
            var result = json[UsCensusBureauConstants.ResultKey];
            return result[UsCensusBureauConstants.AddressMatchesKey]
                .Select(match =>
	            {
		            var matched = match[UsCensusBureauConstants.MatchedAddressKey].ToString();
                    var coordinates = match[UsCensusBureauConstants.CoordinatesKey];
                    var x = double.Parse(coordinates[UsCensusBureauConstants.XKey].ToString());
                    var y = double.Parse(coordinates[UsCensusBureauConstants.YKey].ToString());
                    
                    return new UsCensusBureauAddress(matched, new Location(y, x));
                })
                .ToArray();
        }
    }
}
