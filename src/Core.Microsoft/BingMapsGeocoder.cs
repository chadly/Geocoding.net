using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

using GeoCoding;
using GeoAddress = GeoCoding.Address;
using GeoLocation = GeoCoding.Location;

namespace GeoCoding.Microsoft
{
	public class BingMapsGeoCoder : IGeoCoder
	{
		private string BingKey;
		private static string UNFORMATTED_QUERY = "http://dev.virtualearth.net/REST/v1/Locations/{0}?key={1}";
		private static string FORMATTED_QUERY = "http://dev.virtualearth.net/REST/v1/Locations?CountryRegion={0}&adminDistrict={1}&postalCode={2}&locality={3}&addressLine1={4}&key={5}";

		public BingMapsGeoCoder(string bingKey)
		{
			this.BingKey = bingKey;
		}

		#region IGeoCoder Members

		public GeoAddress[] GeoCode(string address)
		{
			var response = GetResponse(string.Format(UNFORMATTED_QUERY, address, BingKey));
			return ParseResponse(response);
		}

		public GeoAddress[] GeoCode(string street, string city, string state, string postalCode, string country)
		{
			var response = GetResponse(string.Format(FORMATTED_QUERY, country, state, postalCode, city, street, BingKey));
			return ParseResponse(response);
		}

		#endregion

		private GeoAddress[] ParseResponse(Response response)
		{
			List<GeoAddress> addresses = new List<GeoAddress>();
			foreach (var resource in response.ResourceSets[0].Resources)
			{
				addresses.Add(AddressFromBingMaps(resource as Location));
			}
			return addresses.ToArray();
		}

		private Response GetResponse(string queryURL)
		{
			HttpWebRequest request = WebRequest.Create(queryURL) as HttpWebRequest;
			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Response));
				return jsonSerializer.ReadObject(response.GetResponseStream()) as Response;
			}
		}

		private GeoAddress AddressFromBingMaps(Location location)
		{
			return new GeoAddress(
				location.Address.AddressLine,
				location.Address.Locality,
				location.Address.AdminDistrict,
				location.Address.PostalCode,
				location.Address.CountryRegion,
				LocationFromBingMaps(location.Point),
				AccuracyFromBingMaps(location.Address),
				ConfidenceFromBingMaps(location.Confidence)
			);
		}

		private GeoLocation LocationFromBingMaps(Point point)
		{
			return new GeoLocation(point.Coordinates[0],point.Coordinates[1]);
		}

		private AddressAccuracy AccuracyFromBingMaps(Address address)
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

		private ConfidenceLevel ConfidenceFromBingMaps(string confidence)
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
	}
}
