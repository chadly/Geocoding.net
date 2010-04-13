using System;
using System.Linq;
using System.Net;
using GeoCoding.Services.VirtualEarth.Token;
using GeoAddress = GeoCoding.Address;
using GeoLocation = GeoCoding.Location;

namespace GeoCoding.Services.VirtualEarth
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

		public GeoAddress[] GeoCode(string address)
		{
			string token = Token();
			var request = new GeocodeRequest() { Query = address, Credentials = new Credentials() { Token = token } };

			var response = geocodeService.Geocode(request);
			return response.Results.Select(r => AddressFromVirtualEarth(r)).ToArray();
		}

		public GeoAddress[] GeoCode(string street, string city, string state, string postalCode, string country)
		{
			return GeoCode(street + " " + city + ", " + state + " " + postalCode + " " + country);
		}

		private string Token()
		{
			var tokenSpec = new TokenSpecification() { ClientIPAddress = LocalIPAddress.Current, TokenValidityDurationMinutes = 480 };
			var response = tokenService.GetClientToken(new GetClientTokenRequest() { specification = tokenSpec });
			return response.GetClientTokenResult;
		}

		private AddressAccuracy AccuracyFromVirtualEarth(Confidence confidence)
		{
			switch (confidence)
			{
				case Confidence.High: return AddressAccuracy.AddressLevel;
				case Confidence.Medium: return AddressAccuracy.CityLevel;
				case Confidence.Low: return AddressAccuracy.Unknown;
				default: return AddressAccuracy.Unknown;
			}
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
			return new GeoAddress(
				result.Address.AddressLine,
				result.Address.Locality,
				result.Address.AdminDistrict,
				result.Address.PostalCode,
				result.Address.CountryRegion,
				LocationFromVirtualEarth(result.Locations),
				AccuracyFromVirtualEarth(result.Confidence)
			);
		}
	}
}