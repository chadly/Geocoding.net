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

		private string Token()
		{
			var tokenSpec = new TokenSpecification() { ClientIPAddress = LocalIPAddress(), TokenValidityDurationMinutes = 480 };
			var response = tokenService.GetClientToken(new GetClientTokenRequest() { specification = tokenSpec });
			return response.GetClientTokenResult;
		}

		private string LocalIPAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily.ToString() == "InterNetwork")
				{
					return ip.ToString();
				}
			}
			return "127.0.0.1";
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
				return GeoLocation.Empty;

			var loc = locations.First();
			return new GeoLocation(loc.Latitude, loc.Longitude);
		}

		private GeoAddress AddressFromVirtualEarth(GeocodeResult result)
		{
			var address = new GeoAddress()
			{
				Street = result.Address.AddressLine,
				City = result.Address.Locality,
				State = result.Address.AdminDistrict,
				PostalCode = result.Address.PostalCode,
				Country = result.Address.CountryRegion
			};

			address.ChangeLocation(LocationFromVirtualEarth(result.Locations), AccuracyFromVirtualEarth(result.Confidence));
			return address;
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

		public void Dispose()
		{
			if (factory != null)
				factory.Dispose();
		}
	}
}