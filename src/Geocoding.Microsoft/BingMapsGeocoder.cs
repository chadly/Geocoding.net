using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;

namespace Geocoding.Microsoft
{
	/// <remarks>
	/// http://msdn.microsoft.com/en-us/library/ff701715.aspx
	/// </remarks>
	public class BingMapsGeocoder : IGeocoder
	{
		const string UNFORMATTED_QUERY = "http://dev.virtualearth.net/REST/v1/Locations/{0}?key={1}";
		const string FORMATTED_QUERY = "http://dev.virtualearth.net/REST/v1/Locations?{0}&key={1}";
		const string QUERY = "q={0}";
		const string COUNTRY = "countryRegion={0}";
		const string ADMIN = "adminDistrict={0}";
		const string ZIP = "postalCode={0}";
		const string CITY = "locality={0}";
		const string ADDRESS = "addressLine={0}";

		readonly string bingKey;

		public IWebProxy Proxy { get; set; }
		public string Culture { get; set; }
		public Location UserLocation { get; set; }
		public Bounds UserMapView { get; set; }
		public IPAddress UserIP { get; set; }

		public BingMapsGeocoder(string bingKey)
		{
			if (string.IsNullOrWhiteSpace(bingKey))
				throw new ArgumentException("bingKey can not be null or empty");

			this.bingKey = bingKey;
		}

		private string GetQueryUrl(string address)
		{
			var parameters = new StringBuilder();
			bool first = true;
			first = AppendParameter(parameters, address, QUERY, first);
			first = AppendGlobalParameters(parameters, first);

			return string.Format(FORMATTED_QUERY, parameters.ToString(), bingKey);
		}

		private string GetQueryUrl(string street, string city, string state, string postalCode, string country)
		{
			StringBuilder parameters = new StringBuilder();
			bool first = true;
			first = AppendParameter(parameters, city, CITY, first);
			first = AppendParameter(parameters, state, ADMIN, first);
			first = AppendParameter(parameters, postalCode, ZIP, first);
			first = AppendParameter(parameters, country, COUNTRY, first);
			first = AppendParameter(parameters, street, ADDRESS, first);
			first = AppendGlobalParameters(parameters, first);

			return string.Format(FORMATTED_QUERY, parameters.ToString(), bingKey);
		}

		private string GetQueryUrl(double latitude, double longitude)
		{
			var builder = new StringBuilder(string.Format(UNFORMATTED_QUERY, string.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude), bingKey));
			AppendGlobalParameters(builder, false);
			return builder.ToString();
		}

		private IEnumerable<KeyValuePair<string, string>> GetGlobalParameters()
		{
			if (!string.IsNullOrEmpty(Culture))
				yield return new KeyValuePair<string, string>("c", Culture);

			if (UserLocation != null)
				yield return new KeyValuePair<string, string>("userLocation", UserLocation.ToString());

			if (UserMapView != null)
				yield return new KeyValuePair<string, string>("userMapView", string.Concat(UserMapView.SouthWest.ToString(), ",", UserMapView.NorthEast.ToString()));

			if (UserIP != null)
				yield return new KeyValuePair<string, string>("userIp", UserIP.ToString());
		}

		private bool AppendGlobalParameters(StringBuilder parameters, bool first)
		{
			var values = GetGlobalParameters().ToArray();

			if (!first) parameters.Append("&");
			parameters.Append(BuildQueryString(values));

			return first && !values.Any();
		}

		private string BuildQueryString(IEnumerable<KeyValuePair<string, string>> parameters)
		{
			var builder = new StringBuilder();
			foreach (var pair in parameters)
			{
				if (builder.Length > 0) builder.Append("&");

				builder.Append(BingUrlEncode(pair.Key));
				builder.Append("=");
				builder.Append(BingUrlEncode(pair.Value));
			}
			return builder.ToString();
		}

		public IEnumerable<BingAddress> Geocode(string address)
		{
			try
			{
				var url = GetQueryUrl(address);
				var response = GetResponse(url);
				return ParseResponse(response);
			}
			catch (Exception ex)
			{
				throw new BingGeocodingException(ex);
			}
		}

		public IEnumerable<BingAddress> Geocode(string street, string city, string state, string postalCode, string country)
		{
			try
			{
				var url = GetQueryUrl(street, city, state, postalCode, country);
				var response = GetResponse(url);
				return ParseResponse(response);
			}
			catch (Exception ex)
			{
				throw new BingGeocodingException(ex);
			}
		}

		public IEnumerable<BingAddress> ReverseGeocode(Location location)
		{
			if (location == null)
				throw new ArgumentNullException("location");

			return ReverseGeocode(location.Latitude, location.Longitude);
		}

		public IEnumerable<BingAddress> ReverseGeocode(double latitude, double longitude)
		{
			try
			{
				var url = GetQueryUrl(latitude, longitude);
				var response = GetResponse(url);
				return ParseResponse(response);
			}
			catch (Exception ex)
			{
				throw new BingGeocodingException(ex);
			}
		}

		IEnumerable<Address> IGeocoder.Geocode(string address)
		{
			return Geocode(address).Cast<Address>();
		}

		IEnumerable<Address> IGeocoder.Geocode(string street, string city, string state, string postalCode, string country)
		{
			return Geocode(street, city, state, postalCode, country).Cast<Address>();
		}

		IEnumerable<Address> IGeocoder.ReverseGeocode(Location location)
		{
			return ReverseGeocode(location).Cast<Address>();
		}

		IEnumerable<Address> IGeocoder.ReverseGeocode(double latitude, double longitude)
		{
			return ReverseGeocode(latitude, longitude).Cast<Address>();
		}

		private bool AppendParameter(StringBuilder sb, string parameter, string format, bool first)
		{
			if (!string.IsNullOrEmpty(parameter))
			{
				if (!first)
				{
					sb.Append('&');
				}
				sb.Append(string.Format(format, BingUrlEncode(parameter)));
				return false;
			}
			return first;
		}

		private IEnumerable<BingAddress> ParseResponse(Json.Response response)
		{
			var list = new List<BingAddress>();

			foreach (Json.Location location in response.ResourceSets[0].Resources)
			{
				list.Add(new BingAddress(
					location.Address.FormattedAddress,
					new Location(location.Point.Coordinates[0], location.Point.Coordinates[1]),
					location.Address.AddressLine,
					location.Address.AdminDistrict,
					location.Address.AdminDistrict2,
					location.Address.CountryRegion,
					location.Address.Locality,
					location.Address.PostalCode,
					(EntityType)Enum.Parse(typeof(EntityType), location.EntityType),
					EvaluateConfidence(location.Confidence)
				));
			}

			return list;
		}

		private HttpRequestMessage CreateRequest(string url)
		{
			return new HttpRequestMessage(HttpMethod.Get, url);
		}

		HttpClient BuildClient()
		{
			if (this.Proxy == null)
				return new HttpClient();

			var handler = new HttpClientHandler();
			handler.Proxy = this.Proxy;
			return new HttpClient(handler);
		}

		private Json.Response GetResponse(string queryURL)
		{
			using (var client = BuildClient())
			{
				var response = client.SendAsync(CreateRequest(queryURL)).Result;
				return response.Content.ReadAsAsync<Json.Response>().Result;
			}
		}

		private ConfidenceLevel EvaluateConfidence(string confidence)
		{
			switch (confidence.ToLower())
			{
				case "low":
					return ConfidenceLevel.Low;
				case "medium":
					return ConfidenceLevel.Medium;
				case "high":
					return ConfidenceLevel.High;
				default:
					return ConfidenceLevel.Unknown;
			}
		}

		private string BingUrlEncode(string toEncode)
		{
			if (string.IsNullOrEmpty(toEncode))
				return string.Empty;

			return WebUtility.UrlEncode(toEncode);
		}
	}
}
