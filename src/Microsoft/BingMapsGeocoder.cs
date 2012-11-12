using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace GeoCoding.Microsoft
{
	/// <remarks>
	/// http://msdn.microsoft.com/en-us/library/ff701715.aspx
	/// </remarks>
	public class BingMapsGeoCoder : IGeoCoder
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

		public BingMapsGeoCoder(string bingKey)
		{
			this.bingKey = bingKey;
		}

		public IEnumerable<BingAddress> GeoCode(string address)
		{
			try
			{
				var parameters = new StringBuilder();
				AppendParameter(parameters, address, QUERY, true);

				var response = GetResponse(string.Format(FORMATTED_QUERY, parameters.ToString(), bingKey));
				return ParseResponse(response);
			}
			catch (Exception ex)
			{
				throw new BingGeoCodingException(ex);
			}
		}

		public IEnumerable<BingAddress> GeoCode(string street, string city, string state, string postalCode, string country)
		{
			try
			{
				StringBuilder parameters = new StringBuilder();
				bool first = true;
				first = AppendParameter(parameters, city, CITY, first);
				first = AppendParameter(parameters, state, ADMIN, first);
				first = AppendParameter(parameters, postalCode, ZIP, first);
				first = AppendParameter(parameters, country, COUNTRY, first);
				first = AppendParameter(parameters, street, ADDRESS, first);

				var response = GetResponse(
					string.Format(
						FORMATTED_QUERY,
						parameters.ToString(),
						bingKey));
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
				var response = GetResponse(string.Format(UNFORMATTED_QUERY, String.Format("{0},{1}", latitude, longitude), bingKey));
				return ParseResponse(response);
			}
			catch (Exception ex)
			{
				throw new BingGeoCodingException(ex);
			}
		}

		IEnumerable<Address> IGeoCoder.GeoCode(string address)
		{
			return GeoCode(address).Cast<Address>();
		}

		IEnumerable<Address> IGeoCoder.GeoCode(string street, string city, string state, string postalCode, string country)
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
					(EntityType)Enum.Parse(typeof(EntityType), location.EntityType),
					EvaluateConfidence(location.Confidence)
				);
			}
		}

		private Json.Response GetResponse(string queryURL)
		{
			HttpWebRequest request = WebRequest.Create(queryURL) as HttpWebRequest;
			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Json.Response));
				return jsonSerializer.ReadObject(response.GetResponseStream()) as Json.Response;
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
	}
}