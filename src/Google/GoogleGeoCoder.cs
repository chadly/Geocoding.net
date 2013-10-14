using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.XPath;

namespace GeoCoding.Google
{
	/// <remarks>
	/// http://code.google.com/apis/maps/documentation/geocoding/
	/// </remarks>
	public class GoogleGeoCoder : IGeoCoder
	{
		public bool UseSsl { get; set; }

		public string ServiceUrl
		{
			get { return (UseSsl ? "https:" : "http:") + "//maps.googleapis.com/maps/api/geocode/xml?{0}={1}&sensor=false"; }
		}

		public IEnumerable<GoogleAddress> GeoCode(string address)
		{
			if (String.IsNullOrEmpty(address))
				throw new ArgumentNullException("address");

			HttpWebRequest request = BuildWebRequest("address", HttpUtility.UrlEncode(address));
			return ProcessRequest(request);
		}

		public IEnumerable<GoogleAddress> ReverseGeoCode(Location location)
		{
			if (location == null)
				throw new ArgumentNullException("location");

			return ReverseGeoCode(location.Latitude, location.Longitude);
		}

		public IEnumerable<GoogleAddress> ReverseGeoCode(double latitude, double longitude)
		{
			HttpWebRequest request = BuildWebRequest("latlng", String.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude));
			return ProcessRequest(request);
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
			catch (GoogleGeoCodingException)
			{
				//let these pass through
				throw;
			}
			catch (Exception ex)
			{
				//wrap in google exception
				throw new GoogleGeoCodingException(ex);
			}
		}

		IEnumerable<Address> IGeoCoder.GeoCode(string address)
		{
			return GeoCode(address).Cast<Address>();
		}

		IEnumerable<Address> IGeoCoder.GeoCode(string street, string city, string state, string postalCode, string country)
		{
			string address = String.Format("{0} {1}, {2} {3}, {4}", street, city, state, postalCode, country);
			return GeoCode(address).Cast<Address>();
		}

		IEnumerable<Address> IGeoCoder.ReverseGeocode(Location location)
		{
			return ReverseGeoCode(location).Cast<Address>();
		}

		IEnumerable<Address> IGeoCoder.ReverseGeocode(double latitude, double longitude)
		{
			return ReverseGeoCode(latitude, longitude).Cast<Address>();
		}

		private HttpWebRequest BuildWebRequest(string type, string value)
		{
			string url = String.Format(ServiceUrl, type, value);
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.Method = "GET";
			return req;
		}

		private IEnumerable<GoogleAddress> ProcessWebResponse(WebResponse response)
		{
			XPathDocument xmlDoc = LoadXmlResponse(response);
			XPathNavigator nav = xmlDoc.CreateNavigator();

			GoogleStatus status = EvaluateStatus((string)nav.Evaluate("string(/GeocodeResponse/status)"));

			if (status != GoogleStatus.Ok && status != GoogleStatus.ZeroResults)
				throw new GoogleGeoCodingException(status);

			if (status == GoogleStatus.Ok)
				return ParseAddresses(nav.Select("/GeocodeResponse/result"));

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

				var components = ParseComponents(nav.Select("address_component"));

				double latitude = (double)nav.Evaluate("number(geometry/location/lat)");
				double longitude = (double)nav.Evaluate("number(geometry/location/lng)");
				Location coordinates = new Location(latitude, longitude);

				bool isPartialMatch;
				bool.TryParse((string)nav.Evaluate("string(partial_match)"), out isPartialMatch);

				yield return new GoogleAddress(type, formattedAddress, components.ToArray(), coordinates, isPartialMatch);
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
	}
}