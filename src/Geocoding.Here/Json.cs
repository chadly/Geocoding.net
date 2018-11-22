using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Geocoding.Here.Json
{
	[DataContract]
	public class ServerResponse
	{
		[DataMember(Name = "Response")]
		public Response Response { get; set; }
		[DataMember(Name = "Details")]
		public string Details { get; set; }
		[DataMember(Name = "type")]
		public string ErrorType { get; set; }
		[DataMember(Name = "subtype")]
		public string ErrorSubtype { get; set; }
	}

	[DataContract]
    public class Response
	{
		[DataMember(Name = "View")]
        public View[] View { get; set; }
	}

	[DataContract]
	public class View
	{
		[DataMember(Name = "ViewId")]
		public int ViewId { get; set; }
		[DataMember(Name = "Result")]
		public Result[] Result { get; set; }
	}

	[DataContract]
	public class Result
	{
		[DataMember(Name = "Relevance")]
		public float Relevance { get; set; }
		[DataMember(Name = "MatchLevel")]
		public string MatchLevel { get; set; }
		[DataMember(Name = "MatchType")]
		public string MatchType { get; set; }
		[DataMember(Name = "Location")]
		public Location Location { get; set; }
	}

	[DataContract]
	public class Location
	{
		[DataMember(Name = "LocationId")]
		public string LocationId { get; set; }
		[DataMember(Name = "LocationType")]
		public string LocationType { get; set; }
		[DataMember(Name = "Name")]
		public string Name { get; set; }
		[DataMember(Name = "DisplayPosition")]
		public GeoCoordinate DisplayPosition { get; set; }
		[DataMember(Name = "NavigationPosition")]
		public GeoCoordinate NavigationPosition { get; set; }
		[DataMember(Name = "Address")]
		public Address Address { get; set; }
	}

	[DataContract]
	public class GeoCoordinate
	{
		[DataMember(Name = "Latitude")]
		public double Latitude { get; set; }
		[DataMember(Name = "Longitude")]
		public double Longitude { get; set; }
	}

	[DataContract]
	public class GeoBoundingBox
	{
		[DataMember(Name = "TopLeft")]
		public GeoCoordinate TopLeft { get; set; }
		[DataMember(Name = "BottomRight")]
		public GeoCoordinate BottomRight { get; set; }
	}

	[DataContract]
	public class Address
	{
		[DataMember(Name = "Label")]
		public string Label { get; set; }
		[DataMember(Name = "Country")]
		public string Country { get; set; }
		[DataMember(Name = "State")]
		public string State { get; set; }
		[DataMember(Name = "County")]
		public string County { get; set; }
		[DataMember(Name = "City")]
		public string City { get; set; }
		[DataMember(Name = "District")]
		public string District { get; set; }
		[DataMember(Name = "Subdistrict")]
		public string Subdistrict { get; set; }
		[DataMember(Name = "Street")]
		public string Street { get; set; }
		[DataMember(Name = "HouseNumber")]
		public string HouseNumber { get; set; }
		[DataMember(Name = "PostalCode")]
		public string PostalCode { get; set; }
		[DataMember(Name = "Building")]
		public string Building { get; set; }
	}
}
