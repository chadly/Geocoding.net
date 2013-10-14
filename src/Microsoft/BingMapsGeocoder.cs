﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Threading;

namespace GeoCoding.Microsoft
{
	/// <remarks>
	/// http://msdn.microsoft.com/en-us/library/ff701715.aspx
	/// </remarks>
	public class BingMapsGeoCoder : IGeoCoder, IAsyncGeoCoder
	{
		private const string UNFORMATTED_QUERY = "http://dev.virtualearth.net/REST/v1/Locations/{0}?key={1}";
		private const string FORMATTED_QUERY = "http://dev.virtualearth.net/REST/v1/Locations?{0}&key={1}";
		private const string QUERY = "q={0}";
		private const string COUNTRY = "countryRegion={0}";
		private const string ADMIN = "adminDistrict={0}";
		private const string ZIP = "postalCode={0}";
		private const string CITY = "locality={0}";
		private const string ADDRESS = "addressLine={0}";

		private readonly string bingKey;

		public WebProxy Proxy { get; set; }
		public string Culture { get; set; }
		public Location UserLocation { get; set; }
		public Bounds UserMapView { get; set; }
		public IPAddress UserIP { get; set; }

		public BingMapsGeoCoder(string bingKey)
		{
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
			var builder = new StringBuilder(string.Format(UNFORMATTED_QUERY, String.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude), bingKey));
			AppendGlobalParameters(builder, false);
			return builder.ToString();
		}

		private IEnumerable<KeyValuePair<string, string>> GetGlobalParameters()
		{
			if (!String.IsNullOrEmpty(Culture))
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

		public IEnumerable<BingAddress> GeoCode(string address)
		{
			try
			{
				var url = GetQueryUrl(address);
				var response = GetResponse(url);
				return ParseResponse(response);
			}
			catch (Exception ex)
			{
				throw new BingGeoCodingException(ex);
			}
		}

		private HttpWebRequest CreateRequest(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Proxy = Proxy;
			return request;
		}

		public IEnumerable<BingAddress> GeoCode(string street, string city, string state, string postalCode,
			string country)
		{
			try
			{
				var url = GetQueryUrl(street, city, state, postalCode, country);
				var response = GetResponse(url);
				return ParseResponse(response);
			}
			catch (Exception ex)
			{
				throw new BingGeoCodingException(ex);
			}
		}

		public IEnumerable<BingAddress> ReverseGeoCode(Location location)
		{
			if (location == null)
				throw new ArgumentNullException("location");

			return ReverseGeoCode(location.Latitude, location.Longitude);
		}

		public IEnumerable<BingAddress> ReverseGeoCode(double latitude, double longitude)
		{
			try
			{
				var url = GetQueryUrl(latitude, longitude);
				var response = GetResponse(url);
				return ParseResponse(response);
			}
			catch (Exception ex)
			{
				throw new BingGeoCodingException(ex);
			}
		}

		public Task<IEnumerable<BingAddress>> GeoCodeAsync(string address)
		{
			var url = GetQueryUrl(address);
			return GetResponseAsync(url)
				.ContinueWith(task => ParseResponse(task.Result));
		}

		public Task<IEnumerable<BingAddress>> GeoCodeAsync(string address, CancellationToken cancellationToken)
		{
			var url = GetQueryUrl(address);
			return GetResponseAsync(url, cancellationToken)
				.ContinueWith(task => ParseResponse(task.Result), cancellationToken);
		}

		public Task<IEnumerable<BingAddress>> GeoCodeAsync(string street, string city, string state, string postalCode,
			string country)
		{
			var url = GetQueryUrl(street, city, state, postalCode, country);
			return GetResponseAsync(url)
				.ContinueWith(task => ParseResponse(task.Result));
		}

		public Task<IEnumerable<BingAddress>> GeoCodeAsync(string street, string city, string state, string postalCode,
			string country, CancellationToken cancellationToken)
		{
			var url = GetQueryUrl(street, city, state, postalCode, country);
			return GetResponseAsync(url, cancellationToken)
				.ContinueWith(task => ParseResponse(task.Result), cancellationToken);
		}

		public Task<IEnumerable<BingAddress>> ReverseGeocodeAsync(double latitude, double longitude)
		{
			var url = GetQueryUrl(latitude, longitude);
			return GetResponseAsync(url)
				.ContinueWith(task => ParseResponse(task.Result));
		}

		public Task<IEnumerable<BingAddress>> ReverseGeocodeAsync(double latitude, double longitude,
			CancellationToken cancellationToken)
		{
			var url = GetQueryUrl(latitude, longitude);
			return GetResponseAsync(url, cancellationToken)
				.ContinueWith(task => ParseResponse(task.Result), cancellationToken);
		}

		IEnumerable<Address> IGeoCoder.GeoCode(string address)
		{
			return GeoCode(address).Cast<Address>();
		}

		IEnumerable<Address> IGeoCoder.GeoCode(string street, string city, string state, string postalCode,
			string country)
		{
			return GeoCode(street, city, state, postalCode, country).Cast<Address>();
		}

		IEnumerable<Address> IGeoCoder.ReverseGeocode(Location location)
		{
			return ReverseGeoCode(location).Cast<Address>();
		}

		IEnumerable<Address> IGeoCoder.ReverseGeocode(double latitude, double longitude)
		{
			return ReverseGeoCode(latitude, longitude).Cast<Address>();
		}

		Task<IEnumerable<Address>> IAsyncGeoCoder.GeoCodeAsync(string address)
		{
			return GeoCodeAsync(address)
				.ContinueWith(task => task.Result.Cast<Address>());
		}

		Task<IEnumerable<Address>> IAsyncGeoCoder.GeoCodeAsync(string address, CancellationToken cancellationToken)
		{
			return GeoCodeAsync(address, cancellationToken)
				.ContinueWith(task => task.Result.Cast<Address>(), cancellationToken);
		}

		Task<IEnumerable<Address>> IAsyncGeoCoder.GeoCodeAsync(string street, string city, string state,
			string postalCode, string country)
		{
			return GeoCodeAsync(street, city, state, postalCode, country)
				.ContinueWith(task => task.Result.Cast<Address>());
		}

		Task<IEnumerable<Address>> IAsyncGeoCoder.GeoCodeAsync(string street, string city, string state,
			string postalCode, string country, CancellationToken cancellationToken)
		{
			return GeoCodeAsync(street, city, state, postalCode, country, cancellationToken)
				.ContinueWith(task => task.Result.Cast<Address>(), cancellationToken);
		}

		Task<IEnumerable<Address>> IAsyncGeoCoder.ReverseGeocodeAsync(double latitude, double longitude)
		{
			return ReverseGeocodeAsync(latitude, longitude)
				.ContinueWith(task => task.Result.Cast<Address>());
		}

		Task<IEnumerable<Address>> IAsyncGeoCoder.ReverseGeocodeAsync(double latitude, double longitude,
			CancellationToken cancellationToken)
		{
			return ReverseGeocodeAsync(latitude, longitude, cancellationToken)
				.ContinueWith(task => task.Result.Cast<Address>(), cancellationToken);
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
			foreach (Json.Location location in response.ResourceSets[0].Resources)
			{
				yield return new BingAddress(
					location.Address.FormattedAddress,
					new Location(location.Point.Coordinates[0], location.Point.Coordinates[1]),
					location.Address.AddressLine,
					location.Address.AdminDistrict,
					location.Address.AdminDistrict2,
					location.Address.CountryRegion,
					location.Address.Locality,
					location.Address.PostalCode,
					(EntityType) Enum.Parse(typeof (EntityType), location.EntityType),
					EvaluateConfidence(location.Confidence)
					);
			}
		}

		private Json.Response GetResponse(string queryURL)
		{
			using (HttpWebResponse response = CreateRequest(queryURL).GetResponse() as HttpWebResponse)
			{
				DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof (Json.Response));
				return jsonSerializer.ReadObject(response.GetResponseStream()) as Json.Response;
			}
		}

		private Task<Json.Response> GetResponseAsync(string queryURL, CancellationToken? cancellationToken = null)
		{
			HttpWebRequest request = CreateRequest(queryURL);

			if (cancellationToken != null)
			{
				cancellationToken.Value.ThrowIfCancellationRequested();
				cancellationToken.Value.Register(() => request.Abort());
			}

			var requestState = new RequestState(request, cancellationToken);
			return Task.Factory.FromAsync(
				(callback, asyncState) => SendRequestAsync((RequestState) asyncState, callback),
				result => ProcessResponseAsync((RequestState) result.AsyncState, result),
				requestState
				);
		}

		private IAsyncResult SendRequestAsync(RequestState requestState, AsyncCallback callback)
		{
			try
			{
				return requestState.request.BeginGetResponse(callback, requestState);
			}
			catch (Exception ex)
			{
				throw new BingGeoCodingException(ex);
			}
		}

		private Json.Response ProcessResponseAsync(RequestState requestState, IAsyncResult result)
		{
			if (requestState.cancellationToken != null)
				requestState.cancellationToken.Value.ThrowIfCancellationRequested();

			try
			{
				using (var response = (HttpWebResponse) requestState.request.EndGetResponse(result))
				{
					DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof (Json.Response));
					return jsonSerializer.ReadObject(response.GetResponseStream()) as Json.Response;
				}
			}
			catch (BingGeoCodingException)
			{
				//let these pass through
				throw;
			}
			catch (Exception ex)
			{
				//wrap in google exception
				throw new BingGeoCodingException(ex);
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

			return HttpUtility.UrlEncode(toEncode);
		}

		protected class RequestState
		{
			public readonly HttpWebRequest request;
			public readonly CancellationToken? cancellationToken;

			public RequestState(HttpWebRequest request, CancellationToken? cancellationToken)
			{
				if (request == null) throw new ArgumentNullException("request");

				this.request = request;
				this.cancellationToken = cancellationToken;
			}
		}
	}
}