using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Geocoding.Here
{
	/// <remarks>
	/// https://developer.here.com/documentation/geocoder/topics/request-constructing.html
	/// </remarks>
	public class HereGeocoder : IGeocoder
	{
		const string GEOCODING_QUERY = "https://geocoder.api.here.com/6.2/geocode.json?app_id={0}&app_code={1}&{2}";
		const string REVERSE_GEOCODING_QUERY = "https://reverse.geocoder.api.here.com/6.2/reversegeocode.json?app_id={0}&app_code={1}&mode=retrieveAddresses&{2}";
		const string SEARCHTEXT = "searchtext={0}";
		const string PROX = "prox={0}";
		const string STREET = "street={0}";
		const string CITY = "city={0}";
		const string STATE = "state={0}";
		const string POSTAL_CODE = "postalcode={0}";
		const string COUNTRY = "country={0}";

		readonly string appId;
		readonly string appCode;

		public IWebProxy Proxy { get; set; }
		public Location UserLocation { get; set; }
		public Bounds UserMapView { get; set; }
		public int? MaxResults { get; set; }

		public HereGeocoder(string appId, string appCode)
		{
			if (string.IsNullOrWhiteSpace(appId))
				throw new ArgumentException("appId can not be null or empty");

			if (string.IsNullOrWhiteSpace(appCode))
				throw new ArgumentException("appCode can not be null or empty");

			this.appId = appId;
			this.appCode = appCode;
		}

		private string GetQueryUrl(string address)
		{
			var parameters = new StringBuilder();
			var first = AppendParameter(parameters, address, SEARCHTEXT, true);
			AppendGlobalParameters(parameters, first);

			return string.Format(GEOCODING_QUERY, appId, appCode, parameters.ToString());
		}

		private string GetQueryUrl(string street, string city, string state, string postalCode, string country)
		{
			var parameters = new StringBuilder();
			var first = AppendParameter(parameters, street, STREET, true);
			first = AppendParameter(parameters, city, CITY, first);
			first = AppendParameter(parameters, state, STATE, first);
			first = AppendParameter(parameters, postalCode, POSTAL_CODE, first);
			first = AppendParameter(parameters, country, COUNTRY, first);
			AppendGlobalParameters(parameters, first);

			return string.Format(GEOCODING_QUERY, appId, appCode, parameters.ToString());
		}

		private string GetQueryUrl(double latitude, double longitude)
		{
			var parameters = new StringBuilder();
			var first = AppendParameter(parameters, string.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude), PROX, true);
			AppendGlobalParameters(parameters, first);

			return string.Format(REVERSE_GEOCODING_QUERY, appId, appCode, parameters.ToString());
		}

		private IEnumerable<KeyValuePair<string, string>> GetGlobalParameters()
		{
			if (UserLocation != null)
				yield return new KeyValuePair<string, string>("prox", UserLocation.ToString());

			if (UserMapView != null)
				yield return new KeyValuePair<string, string>("mapview", string.Concat(UserMapView.SouthWest.ToString(), ",", UserMapView.NorthEast.ToString()));
			
			if (MaxResults != null && MaxResults.Value > 0)
				yield return new KeyValuePair<string, string>("maxresults", MaxResults.Value.ToString(CultureInfo.InvariantCulture));
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

				builder.Append(UrlEncode(pair.Key));
				builder.Append("=");
				builder.Append(UrlEncode(pair.Value));
			}
			return builder.ToString();
		}

		public async Task<IEnumerable<HereAddress>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var url = GetQueryUrl(address);
				var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
				return ParseResponse(response);
			}
			catch (Exception ex)
			{
				throw new HereGeocodingException(ex);
			}
		}

		public async Task<IEnumerable<HereAddress>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var url = GetQueryUrl(street, city, state, postalCode, country);
				var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
				return ParseResponse(response);
			}
			catch (Exception ex)
			{
				throw new HereGeocodingException(ex);
			}
		}

		public async Task<IEnumerable<HereAddress>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (location == null)
				throw new ArgumentNullException(nameof(location));

			return await ReverseGeocodeAsync(location.Latitude, location.Longitude, cancellationToken).ConfigureAwait(false);
		}

		public async Task<IEnumerable<HereAddress>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var url = GetQueryUrl(latitude, longitude);
				var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
				return ParseResponse(response);
			}
			catch (Exception ex)
			{
				throw new HereGeocodingException(ex);
			}
		}

		async Task<IEnumerable<Address>> IGeocoder.GeocodeAsync(string address, CancellationToken cancellationToken)
		{
			return await GeocodeAsync(address, cancellationToken).ConfigureAwait(false);
		}

		async Task<IEnumerable<Address>> IGeocoder.GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken)
		{
			return await GeocodeAsync(street, city, state, postalCode, country, cancellationToken).ConfigureAwait(false);
		}

		async Task<IEnumerable<Address>> IGeocoder.ReverseGeocodeAsync(Location location, CancellationToken cancellationToken)
		{
			return await ReverseGeocodeAsync(location, cancellationToken).ConfigureAwait(false);
		}

		async Task<IEnumerable<Address>> IGeocoder.ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken)
		{
			return await ReverseGeocodeAsync(latitude, longitude, cancellationToken).ConfigureAwait(false);
		}

		private bool AppendParameter(StringBuilder sb, string parameter, string format, bool first)
		{
			if (!string.IsNullOrEmpty(parameter))
			{
				if (!first)
				{
					sb.Append('&');
				}
				sb.Append(string.Format(format, UrlEncode(parameter)));
				return false;
			}
			return first;
		}

		private IEnumerable<HereAddress> ParseResponse(Json.Response response)
		{
			foreach (var view in response.View)
			{
				foreach (var result in view.Result)
				{
					var location = result.Location;
					yield return new HereAddress(
						location.Address.Label,
						new Location(location.DisplayPosition.Latitude, location.DisplayPosition.Longitude),
						location.Address.Street,
						location.Address.HouseNumber,
						location.Address.City,
						location.Address.State,
						location.Address.PostalCode,
						location.Address.Country,
						(HereLocationType)Enum.Parse(typeof(HereLocationType), location.LocationType, true));
				}
			}
		}

		private HttpRequestMessage CreateRequest(string url)
		{
			return new HttpRequestMessage(HttpMethod.Get, url);
		}

		private HttpClient BuildClient()
		{
			if (this.Proxy == null)
				return new HttpClient();

			var handler = new HttpClientHandler {Proxy = this.Proxy};
			return new HttpClient(handler);
		}

		private async Task<Json.Response> GetResponse(string queryURL, CancellationToken cancellationToken)
		{
			using (var client = BuildClient())
			{
				var response = await client.SendAsync(CreateRequest(queryURL), cancellationToken).ConfigureAwait(false);
				using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
				{
					var jsonSerializer = new DataContractJsonSerializer(typeof(Json.ServerResponse));
					var serverResponse = (Json.ServerResponse)jsonSerializer.ReadObject(stream);

					return serverResponse.Response;
				}
			}
		}

		private string UrlEncode(string toEncode)
		{
			if (string.IsNullOrEmpty(toEncode))
				return string.Empty;

			return WebUtility.UrlEncode(toEncode);
		}
	}
}
