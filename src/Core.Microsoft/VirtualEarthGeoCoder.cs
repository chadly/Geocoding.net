using System;
using System.Linq;
using System.Net;
using GeoCoding.Microsoft.VirtualEarth;
using GeoCoding.Microsoft.VirtualEarth.Token;
using GeoAddress = GeoCoding.Address;
using GeoLocation = GeoCoding.Location;
using System.Collections.Generic;

namespace GeoCoding.Microsoft
{
	public class VirtualEarthGeoCoder : IGeoCoder, IDisposable
	{
		private readonly VirtualEarthServiceFactory factory;
		private readonly IGeocodeService geocodeService;
		private readonly CommonServiceSoap tokenService;

		public VirtualEarthGeoCoder(string username, string password)
			: this(username, password, false) { }

		public VirtualEarthGeoCoder(string username, string password, bool useStaging)
		{
			this.factory = new VirtualEarthServiceFactory(username, password, useStaging);
			this.geocodeService = factory.CreateGeocodeService();
			this.tokenService = factory.CreateTokenService();
		}

		public VirtualEarthGeoCoder(IGeocodeService geocodeService, CommonServiceSoap tokenService)
		{
			this.geocodeService = geocodeService;
			this.tokenService = tokenService;
		}

		public void Dispose()
		{
			if (factory != null)
				factory.Dispose();
		}

		public IEnumerable<GeoAddress> GeoCode(string address)
		{
			string token = Token();
			var request = new GeocodeRequest() { Query = address, Credentials = new Credentials() { Token = token } };

			var response = geocodeService.Geocode(request);
			return response.Results.Select(r => AddressFromVirtualEarth(r)).ToArray();
		}

		public IEnumerable<GeoAddress> GeoCode(string street, string city, string state, string postalCode, string country)
		{
			return GeoCode(street + " " + city + ", " + state + " " + postalCode + " " + country);
		}

		private string Token()
		{
			var tokenSpec = new TokenSpecification() { ClientIPAddress = LocalIPAddress.Current, TokenValidityDurationMinutes = 480 };
			var response = tokenService.GetClientToken(new GetClientTokenRequest() { specification = tokenSpec });
			return response.GetClientTokenResult;
		}

		private AddressAccuracy AccuracyFromVirtualEarth(VirtualEarth.Address address)
		{
			//Virtual Earth returns an address "confidence" which is not very helpful when trying to determine the accuracy of the address, hence, this:

			if (!String.IsNullOrEmpty(address.AddressLine))
				return AddressAccuracy.AddressLevel;

			if (!String.IsNullOrEmpty(address.PostalCode))
				return AddressAccuracy.PostalCodeLevel;

			if (!String.IsNullOrEmpty(address.Locality))
				return AddressAccuracy.CityLevel;

			if (!String.IsNullOrEmpty(address.AdminDistrict))
				return AddressAccuracy.StateLevel;

			if (!String.IsNullOrEmpty(address.CountryRegion))
				return AddressAccuracy.CountryLevel;

			return AddressAccuracy.Unknown;
		}

		private GeoLocation LocationFromVirtualEarth(GeocodeLocation[] locations)
		{
			if (!locations.Any())
				return null;

			var loc = locations.First();
			return new GeoLocation(loc.Latitude, loc.Longitude);
		}

		private GeoAddress AddressFromVirtualEarth(GeocodeResult result)
		{
#warning need to fix this
			return null;
			//return new GeoAddress(
			//    result.Address.AddressLine,
			//    result.Address.Locality,
			//    result.Address.AdminDistrict,
			//    result.Address.PostalCode,
			//    result.Address.CountryRegion,
			//    LocationFromVirtualEarth(result.Locations),
			//    AccuracyFromVirtualEarth(result.Address),
			//    ConfidenceFromVirtualEarth(result.Confidence)
			//);
		}

		private ConfidenceLevel ConfidenceFromVirtualEarth(VirtualEarth.Confidence confidence)
		{
			switch (confidence)
			{
				case VirtualEarth.Confidence.High:
					return ConfidenceLevel.High;
				case VirtualEarth.Confidence.Low:
					return ConfidenceLevel.Low;
				case VirtualEarth.Confidence.Medium:
					return ConfidenceLevel.Medium;
				default:
					return ConfidenceLevel.Unknown;
			}
		}
	}
}