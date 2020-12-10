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
		const string URL_GEOCODE = "https://geocode.search.hereapi.com/v1/geocode";
		const string URL_REVERSE_GEOCODE = "https://revgeocode.search.hereapi.com/v1/revgeocode";

		const string QN_API_KEY = "apiKey";
		const string QN_LIMIT = "limit";

		const string QN_GEOCODE_QUERY = "q";
		const string QN_GEOCODE_STRUCTURED_QUERY = "qq";

		const string SQN_STREET = "street";
		const string SQN_CITY = "city";
		const string SQN_STATE = "state";
		const string SQN_POSTAL_CODE = "postalCode";
		const string SQN_COUNTRY = "country";

		const string QN_REVERSE_GEOCODE_QUERY = "at";

		readonly string apiKey;

		public IWebProxy Proxy { get; set; }
		public int? MaxResults { get; set; }

		public HereGeocoder(string apiKey)
		{
			if (string.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentException("apiKey can not be null or empty");

			this.apiKey = apiKey;
		}

		private Uri GetQueryUrl(string address)
		{
			UriBuilder uriBuilder = new UriBuilder(URL_GEOCODE);

			QueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.AddParameter(QN_GEOCODE_QUERY, address)
				.AddParameters(GetGlobalParameters());

			uriBuilder.Query = queryBuilder.GetQuery();
			return uriBuilder.Uri;
		}

		private Uri GetQueryUrl(string street, string city, string state, string postalCode, string country)
		{
			UriBuilder uriBuilder = new UriBuilder(URL_GEOCODE);

			QueryBuilder structuredQueryBuilder = new QueryBuilder();
			structuredQueryBuilder.AddNonEmptyParameter(SQN_STREET, street)
				.AddNonEmptyParameter(SQN_CITY, city)
				.AddNonEmptyParameter(SQN_STATE, state)
				.AddNonEmptyParameter(SQN_POSTAL_CODE, postalCode)
				.AddNonEmptyParameter(SQN_COUNTRY, country);

			QueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.AddParameter(QN_GEOCODE_STRUCTURED_QUERY, structuredQueryBuilder.GetQuery(";"))
				.AddParameters(GetGlobalParameters());

			uriBuilder.Query = queryBuilder.GetQuery();
			return uriBuilder.Uri;
		}

		private Uri GetQueryUrl(double latitude, double longitude)
		{
			UriBuilder uriBuilder = new UriBuilder(URL_REVERSE_GEOCODE);

			QueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.AddParameter(QN_REVERSE_GEOCODE_QUERY, string.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude))
				.AddParameters(GetGlobalParameters());

			uriBuilder.Query = queryBuilder.GetQuery();
			return uriBuilder.Uri;
		}

		private IEnumerable<QueryParameter> GetGlobalParameters()
		{
			if (MaxResults != null && MaxResults.Value > 0)
				yield return new QueryParameter(QN_LIMIT, MaxResults.Value.ToString(CultureInfo.InvariantCulture));

			yield return new QueryParameter(QN_API_KEY, this.apiKey);
		}

		public async Task<IEnumerable<HereAddress>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				Uri url = GetQueryUrl(address);
				var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
				return ParseResponse(response);
			}
			catch (HereGeocodingException)
			{
				throw;
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
			catch (HereGeocodingException)
			{
				throw;
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
			catch (HereGeocodingException)
			{
				throw;
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

		private IEnumerable<HereAddress> ParseResponse(IEnumerable<Json.Item> responseItems)
		{
			foreach (var item in responseItems)
			{
				yield return new HereAddress(
					item.Address.Label,
					new Location(item.Position.Latitude, item.Position.Longitude),
					item.Address.Street,
					item.Address.HouseNumber,
					item.Address.City,
					item.Address.State,
					item.Address.PostalCode,
					item.Address.CountryName,
					item.ResultType);
			}
		}

		private HttpRequestMessage CreateRequest(Uri url)
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

		private async Task<Json.Item[]> GetResponse(Uri url, CancellationToken cancellationToken)
		{
			using (var client = BuildClient())
			{
				HttpResponseMessage response = await client.SendAsync(CreateRequest(url), cancellationToken).ConfigureAwait(false);
				using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
				{
					var jsonSerializer = new DataContractJsonSerializer(typeof(Json.ServerResponse));
					var serverResponse = (Json.ServerResponse)jsonSerializer.ReadObject(stream);

					if (serverResponse.StatusCode != null)
					{
						throw new HereGeocodingException(
							serverResponse.Title, 
							serverResponse.StatusCode.Value, 
							serverResponse.Cause,
							serverResponse.Action,
							serverResponse.CorrelationId,
							serverResponse.RequestId);
					}

					return serverResponse.Items;
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
