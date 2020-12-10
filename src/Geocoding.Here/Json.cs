using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Geocoding.Here.Json
{
	[DataContract]
	public class ServerResponse
	{
		// Successful geocode fields
		[DataMember(Name = "items")]
		public Item[] Items { get; set; }

		// Error fields
		[DataMember(Name = "status")]
		public int? StatusCode { get; set; }
		[DataMember(Name = "title")]
		public string Title { get; set; }
		[DataMember(Name = "cause")]
		public string Cause { get; set; }
		[DataMember(Name = "action")]
		public string Action { get; set; }
		[DataMember(Name = "correlationId")]
		public string CorrelationId { get; set; }
		[DataMember(Name = "requestId")]
		public string RequestId { get; set; }

		// Other error fields
		[DataMember(Name = "error")]
		public string Error { get; set; }
		[DataMember(Name = "error_description")]
		public string ErrorDescription { get; set; }
	}

	[DataContract]
	public class Item
	{
		[DataMember(Name = "title")]
		public String Title { get; set; }
		[DataMember(Name = "id")]
		public String Id { get; set; }
		[DataMember(Name = "resultType")]
		public String ResultType { get; set; }
		[DataMember(Name = "houseNumberType")]
		public String HouseNumberType { get; set; }
		[DataMember(Name = "address")]
		public Address Address { get; set; }
		[DataMember(Name = "position")]
		public GeoCoordinate Position { get; set; }
		[DataMember(Name = "access")]
		public GeoCoordinate[] Access { get; set; }
		[DataMember(Name = "mapView")]
		public GeoBoundingBox MapView { get; set; }
		[DataMember(Name = "scoring")]
		public Scoring Scoring { get; set; }
	}

	[DataContract]
	public class Address
	{
		[DataMember(Name = "label")]
		public string Label { get; set; }
		[DataMember(Name = "countryCode")]
		public string CountryCode { get; set; }
		[DataMember(Name = "countryname")]
		public string CountryName { get; set; }
		[DataMember(Name = "stateCode")]
		public string StateCode { get; set; }
		[DataMember(Name = "state")]
		public string State { get; set; }
		[DataMember(Name = "county")]
		public string County { get; set; }
		[DataMember(Name = "city")]
		public string City { get; set; }
		[DataMember(Name = "cistrict")]
		public string District { get; set; }
		[DataMember(Name = "street")]
		public string Street { get; set; }
		[DataMember(Name = "houseNumber")]
		public string HouseNumber { get; set; }
		[DataMember(Name = "postalCode")]
		public string PostalCode { get; set; }
	}

	[DataContract]
	public class GeoCoordinate
	{
		[DataMember(Name = "lat")]
		public double Latitude { get; set; }
		[DataMember(Name = "lng")]
		public double Longitude { get; set; }
	}

	[DataContract]
	public class GeoBoundingBox
	{
		[DataMember(Name = "west")]
		public double West { get; set; }
		[DataMember(Name = "south")]
		public double South { get; set; }
		[DataMember(Name = "east")]
		public double East { get; set; }
		[DataMember(Name = "north")]
		public double North { get; set; }
	}

	[DataContract]
	public class Scoring
	{
		[DataMember(Name = "queryScore")]
		public double QueryScore { get; set; }
		[DataMember(Name = "fieldScore")]
		public FieldScoring FieldScore { get; set; }
	}

	[DataContract]
	public class FieldScoring
	{
		[DataMember(Name = "city")]
		public double City { get; set; }
		[DataMember(Name = "streets")]
		public double[] Streets { get; set; }
		[DataMember(Name = "houseNumber")]
		public double HouseNumber { get; set; }
	}

}
