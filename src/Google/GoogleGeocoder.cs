using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.XPath;

namespace Geocoding.Google
{
	/// <remarks>
	/// http://code.google.com/apis/maps/documentation/geocoding/
	/// </remarks>
	public class GoogleGeocoder : IGeocoder, IAsyncGeocoder
	{
		string apiKey;
		BusinessKey businessKey;
		const string keyMessage = "Only one of BusinessKey or ApiKey should be set on the GoogleGeocoder.";

		public GoogleGeocoder() { }

		public GoogleGeocoder(BusinessKey businessKey)
		{
			BusinessKey = businessKey;
		}

		public GoogleGeocoder(string apiKey)
		{
			ApiKey = apiKey;
		}

		public string ApiKey
		{
			get { return apiKey; }
			set
			{
				if (businessKey != null)
					throw new InvalidOperationException(keyMessage);
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentException("ApiKey can not be null or empty");

				apiKey = value;
			}
		}

		public BusinessKey BusinessKey
		{
			get { return businessKey; }
			set
			{
				if (!string.IsNullOrEmpty(apiKey))
					throw new InvalidOperationException(keyMessage);
				if (value == null)
					throw new ArgumentException("BusinessKey can not be null");

				businessKey = value;
			}
		}

		public WebProxy Proxy { get; set; }
		public string Language { get; set; }
		public string RegionBias { get; set; }
		public Bounds BoundsBias { get; set; }

		public string ServiceUrl
		{
			get
			{
				var builder = new StringBuilder();
				builder.Append("https://maps.googleapis.com/maps/api/geocode/xml?{0}={1}&sensor=false");

				if (!string.IsNullOrEmpty(Language))
				{
					builder.Append("&language=");
					builder.Append(HttpUtility.UrlEncode(Language));
				}

				if (!string.IsNullOrEmpty(RegionBias))
				{
					builder.Append("&region=");
					builder.Append(HttpUtility.UrlEncode(RegionBias));
				}

				if (!string.IsNullOrEmpty(ApiKey))
				{
					builder.Append("&key=");
					builder.Append(HttpUtility.UrlEncode(ApiKey));
				}

				if (BusinessKey != null)
				{
					builder.Append("&client=");
					builder.Append(HttpUtility.UrlEncode(BusinessKey.ClientId));
				}

				if (BoundsBias != null)
				{
					builder.Append("&bounds=");
					builder.Append(BoundsBias.SouthWest.Latitude.ToString(CultureInfo.InvariantCulture));
					builder.Append(",");
					builder.Append(BoundsBias.SouthWest.Longitude.ToString(CultureInfo.InvariantCulture));
					builder.Append("|");
					builder.Append(BoundsBias.NorthEast.Latitude.ToString(CultureInfo.InvariantCulture));
					builder.Append(",");
					builder.Append(BoundsBias.NorthEast.Longitude.ToString(CultureInfo.InvariantCulture));
				}

				return builder.ToString();
			}
		}

		public IEnumerable<GoogleAddress> Geocode(string address)
		{
			if (string.IsNullOrEmpty(address))
				throw new ArgumentNullException("address");

			HttpWebRequest request = BuildWebRequest("address", HttpUtility.UrlEncode(address));
			return ProcessRequest(request);
		}

		public IEnumerable<GoogleAddress> ReverseGeocode(Location location)
		{
			if (location == null)
				throw new ArgumentNullException("location");

			return ReverseGeocode(location.Latitude, location.Longitude);
		}

		public IEnumerable<GoogleAddress> ReverseGeocode(double latitude, double longitude)
		{
			HttpWebRequest request = BuildWebRequest("latlng", BuildGeolocation(latitude, longitude));
			return ProcessRequest(request);
		}

		public Task<IEnumerable<GoogleAddress>> GeocodeAsync(string address)
		{
			if (string.IsNullOrEmpty(address))
				throw new ArgumentNullException("address");

			HttpWebRequest request = BuildWebRequest("address", HttpUtility.UrlEncode(address));
			return ProcessRequestAsync(request);
		}

		public Task<IEnumerable<GoogleAddress>> GeocodeAsync(string address, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(address))
				throw new ArgumentNullException("address");

			HttpWebRequest request = BuildWebRequest("address", HttpUtility.UrlEncode(address));
			return ProcessRequestAsync(request, cancellationToken);
		}

		public Task<IEnumerable<GoogleAddress>> ReverseGeocodeAsync(double latitude, double longitude)
		{
			HttpWebRequest request = BuildWebRequest("latlng", BuildGeolocation(latitude, longitude));
			return ProcessRequestAsync(request);
		}

		public Task<IEnumerable<GoogleAddress>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken)
		{
			HttpWebRequest request = BuildWebRequest("latlng", BuildGeolocation(latitude, longitude));
			return ProcessRequestAsync(request, cancellationToken);
		}

		private string BuildAddress(string street, string city, string state, string postalCode, string country)
		{
			return string.Format("{0} {1}, {2} {3}, {4}", street, city, state, postalCode, country);
		}

		private string BuildGeolocation(double latitude, double longitude)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude);
		}

		private IEnumerable<GoogleAddress> ProcessRequest(HttpWebRequest request)
		{
			try
			{
				using (WebResponse response = request.GetResponse())
				{
					return ProcessWebResponse(response);
				}
			}
			catch (GoogleGeocodingException)
			{
				//let these pass through
				throw;
			}
			catch (Exception ex)
			{
				//wrap in google exception
				throw new GoogleGeocodingException(ex);
			}
		}

		private Task<IEnumerable<GoogleAddress>> ProcessRequestAsync(HttpWebRequest request, CancellationToken? cancellationToken = null)
		{
			if (cancellationToken != null)
			{
				cancellationToken.Value.ThrowIfCancellationRequested();
				cancellationToken.Value.Register(() => request.Abort());
			}

			var requestState = new RequestState(request, cancellationToken);
			return Task.Factory.FromAsync(
				(callback, asyncState) => SendRequestAsync((RequestState)asyncState, callback),
				result => ProcessResponseAsync((RequestState)result.AsyncState, result),
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
				throw new GoogleGeocodingException(ex);
			}
		}

		private IEnumerable<GoogleAddress> ProcessResponseAsync(RequestState requestState, IAsyncResult result)
		{
			if (requestState.cancellationToken != null)
				requestState.cancellationToken.Value.ThrowIfCancellationRequested();

			try
			{
				using (var response = (HttpWebResponse)requestState.request.EndGetResponse(result))
				{
					return ProcessWebResponse(response);
				}
			}
			catch (GoogleGeocodingException)
			{
				//let these pass through
				throw;
			}
			catch (Exception ex)
			{
				//wrap in google exception
				throw new GoogleGeocodingException(ex);
			}
		}

		IEnumerable<Address> IGeocoder.Geocode(string address)
		{
			return Geocode(address).Cast<Address>();
		}

		IEnumerable<Address> IGeocoder.Geocode(string street, string city, string state, string postalCode, string country)
		{
			return Geocode(BuildAddress(street, city, state, postalCode, country)).Cast<Address>();
		}

		IEnumerable<Address> IGeocoder.ReverseGeocode(Location location)
		{
			return ReverseGeocode(location).Cast<Address>();
		}

		IEnumerable<Address> IGeocoder.ReverseGeocode(double latitude, double longitude)
		{
			return ReverseGeocode(latitude, longitude).Cast<Address>();
		}

		Task<IEnumerable<Address>> IAsyncGeocoder.GeocodeAsync(string address)
		{
			return GeocodeAsync(address)
				.ContinueWith(task => task.Result.Cast<Address>());
		}

		Task<IEnumerable<Address>> IAsyncGeocoder.GeocodeAsync(string address, CancellationToken cancellationToken)
		{
			return GeocodeAsync(address, cancellationToken)
				.ContinueWith(task => task.Result.Cast<Address>(), cancellationToken);
		}

		Task<IEnumerable<Address>> IAsyncGeocoder.GeocodeAsync(string street, string city, string state, string postalCode, string country)
		{
			return GeocodeAsync(BuildAddress(street, city, state, postalCode, country))
				.ContinueWith(task => task.Result.Cast<Address>());
		}

		Task<IEnumerable<Address>> IAsyncGeocoder.GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken)
		{
			return GeocodeAsync(BuildAddress(street, city, state, postalCode, country), cancellationToken)
				.ContinueWith(task => task.Result.Cast<Address>(), cancellationToken);
		}

		Task<IEnumerable<Address>> IAsyncGeocoder.ReverseGeocodeAsync(double latitude, double longitude)
		{
			return ReverseGeocodeAsync(latitude, longitude)
				.ContinueWith(task => task.Result.Cast<Address>());
		}

		Task<IEnumerable<Address>> IAsyncGeocoder.ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken)
		{
			return ReverseGeocodeAsync(latitude, longitude, cancellationToken)
				.ContinueWith(task => task.Result.Cast<Address>(), cancellationToken);
		}

		private HttpWebRequest BuildWebRequest(string type, string value)
		{
			string url = string.Format(ServiceUrl, type, value);

			if (BusinessKey != null)
				url = BusinessKey.GenerateSignature(url);

			var req = WebRequest.Create(url) as HttpWebRequest;
			req.Proxy = Proxy;
			req.Method = "GET";
			return req;
		}

		private IEnumerable<GoogleAddress> ProcessWebResponse(WebResponse response)
		{
			XPathDocument xmlDoc = LoadXmlResponse(response);
			XPathNavigator nav = xmlDoc.CreateNavigator();

			GoogleStatus status = EvaluateStatus((string)nav.Evaluate("string(/GeocodeResponse/status)"));

			if (status != GoogleStatus.Ok && status != GoogleStatus.ZeroResults)
				throw new GoogleGeocodingException(status);

			if (status == GoogleStatus.Ok)
				return ParseAddresses(nav.Select("/GeocodeResponse/result")).ToArray();

			return new GoogleAddress[0];
		}

		private XPathDocument LoadXmlResponse(WebResponse response)
		{
			using (Stream stream = response.GetResponseStream())
			{
				XPathDocument doc = new XPathDocument(stream);
				return doc;
			}
		}

		private IEnumerable<GoogleAddress> ParseAddresses(XPathNodeIterator nodes)
		{
			while (nodes.MoveNext())
			{
				XPathNavigator nav = nodes.Current;

				GoogleAddressType type = EvaluateType((string)nav.Evaluate("string(type)"));
				string formattedAddress = (string)nav.Evaluate("string(formatted_address)");

				var components = ParseComponents(nav.Select("address_component")).ToArray();

				double latitude = (double)nav.Evaluate("number(geometry/location/lat)");
				double longitude = (double)nav.Evaluate("number(geometry/location/lng)");
				Location coordinates = new Location(latitude, longitude);

				double neLatitude = (double)nav.Evaluate("number(geometry/viewport/northeast/lat)");
				double neLongitude = (double)nav.Evaluate("number(geometry/viewport/northeast/lng)");
				Location neCoordinates = new Location(neLatitude, neLongitude);

				double swLatitude = (double)nav.Evaluate("number(geometry/viewport/southwest/lat)");
				double swLongitude = (double)nav.Evaluate("number(geometry/viewport/southwest/lng)");
				Location swCoordinates = new Location(swLatitude, swLongitude);

				var viewport = new GoogleViewport { Northeast = neCoordinates, Southwest = swCoordinates };

            double bounds_neLatitude = (double)nav.Evaluate("number(geometry/bounds/northeast/lat)");
            double bounds_neLongitude = (double)nav.Evaluate("number(geometry/bounds/northeast/lng)");
            Location bounds_neCoordinates = new Location(bounds_neLatitude, bounds_neLongitude);

            double bounds_swLatitude = (double)nav.Evaluate("number(geometry/bounds/southwest/lat)");
            double bounds_swLongitude = (double)nav.Evaluate("number(geometry/bounds/southwest/lng)");
            Location bounds_swCoordinates = new Location(bounds_swLatitude, bounds_swLongitude);

            var bounds = new GoogleBounds { Northeast = bounds_neCoordinates, Southwest = bounds_swCoordinates };

				bool isPartialMatch;
				bool.TryParse((string)nav.Evaluate("string(partial_match)"), out isPartialMatch);

            yield return new GoogleAddress(type, formattedAddress, components, coordinates, viewport, bounds, isPartialMatch);
			}
		}

		private IEnumerable<GoogleAddressComponent> ParseComponents(XPathNodeIterator nodes)
		{
			while (nodes.MoveNext())
			{
				XPathNavigator nav = nodes.Current;

				string longName = (string)nav.Evaluate("string(long_name)");
				string shortName = (string)nav.Evaluate("string(short_name)");
				var types = ParseComponentTypes(nav.Select("type")).ToArray();

				if (types.Any()) //don't return an address component with no type
					yield return new GoogleAddressComponent(types, longName, shortName);
			}
		}

		private IEnumerable<GoogleAddressType> ParseComponentTypes(XPathNodeIterator nodes)
		{
			while (nodes.MoveNext())
				yield return EvaluateType(nodes.Current.InnerXml);
		}

		/// <remarks>
		/// http://code.google.com/apis/maps/documentation/geocoding/#StatusCodes
		/// </remarks>
		private GoogleStatus EvaluateStatus(string status)
		{
			switch (status)
			{
				case "OK": return GoogleStatus.Ok;
				case "ZERO_RESULTS": return GoogleStatus.ZeroResults;
				case "OVER_QUERY_LIMIT": return GoogleStatus.OverQueryLimit;
				case "REQUEST_DENIED": return GoogleStatus.RequestDenied;
				case "INVALID_REQUEST": return GoogleStatus.InvalidRequest;
				default: return GoogleStatus.Error;
			}
		}

		/// <remarks>
		/// http://code.google.com/apis/maps/documentation/geocoding/#Types
		/// </remarks>
		private GoogleAddressType EvaluateType(string type)
		{
			switch (type)
			{
				case "street_address": return GoogleAddressType.StreetAddress;
				case "route": return GoogleAddressType.Route;
				case "intersection": return GoogleAddressType.Intersection;
				case "political": return GoogleAddressType.Political;
				case "country": return GoogleAddressType.Country;
				case "administrative_area_level_1": return GoogleAddressType.AdministrativeAreaLevel1;
				case "administrative_area_level_2": return GoogleAddressType.AdministrativeAreaLevel2;
				case "administrative_area_level_3": return GoogleAddressType.AdministrativeAreaLevel3;
				case "colloquial_area": return GoogleAddressType.ColloquialArea;
				case "locality": return GoogleAddressType.Locality;
				case "sublocality": return GoogleAddressType.SubLocality;
				case "neighborhood": return GoogleAddressType.Neighborhood;
				case "premise": return GoogleAddressType.Premise;
				case "subpremise": return GoogleAddressType.Subpremise;
				case "postal_code": return GoogleAddressType.PostalCode;
				case "natural_feature": return GoogleAddressType.NaturalFeature;
				case "airport": return GoogleAddressType.Airport;
				case "park": return GoogleAddressType.Park;
				case "point_of_interest": return GoogleAddressType.PointOfInterest;
				case "post_box": return GoogleAddressType.PostBox;
				case "street_number": return GoogleAddressType.StreetNumber;
				case "floor": return GoogleAddressType.Floor;
				case "room": return GoogleAddressType.Room;

				default: return GoogleAddressType.Unknown;
			}
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