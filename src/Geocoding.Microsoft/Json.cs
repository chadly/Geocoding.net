using System.Runtime.Serialization;

namespace Geocoding.Microsoft.Json
{
	[DataContract]
	public class Address
	{
		[DataMember(Name = "addressLine")]
		public string AddressLine { get; set; }
		[DataMember(Name = "adminDistrict")]
		public string AdminDistrict { get; set; }
		[DataMember(Name = "adminDistrict2")]
		public string AdminDistrict2 { get; set; }
		[DataMember(Name = "countryRegion")]
		public string CountryRegion { get; set; }
		[DataMember(Name = "formattedAddress")]
		public string FormattedAddress { get; set; }
		[DataMember(Name = "locality")]
		public string Locality { get; set; }
		[DataMember(Name = "neighborhood")]
		public string Neighborhood { get; set; }
		[DataMember(Name = "postalCode")]
		public string PostalCode { get; set; }
	}
	[DataContract]
	public class BoundingBox
	{
		[DataMember(Name = "southLatitude")]
		public double SouthLatitude { get; set; }
		[DataMember(Name = "westLongitude")]
		public double WestLongitude { get; set; }
		[DataMember(Name = "northLatitude")]
		public double NorthLatitude { get; set; }
		[DataMember(Name = "eastLongitude")]
		public double EastLongitude { get; set; }
	}
	[DataContract]
	public class Hint
	{
		[DataMember(Name = "hintType")]
		public string HintType { get; set; }
		[DataMember(Name = "value")]
		public string Value { get; set; }
	}
	[DataContract]
	public class Instruction
	{
		[DataMember(Name = "maneuverType")]
		public string ManeuverType { get; set; }
		[DataMember(Name = "text")]
		public string Text { get; set; }
		//[DataMember(Name = "value")]
		//public string Value { get; set; }
	}
	[DataContract]
	public class ItineraryItem
	{
		[DataMember(Name = "travelMode")]
		public string TravelMode { get; set; }
		[DataMember(Name = "travelDistance")]
		public double TravelDistance { get; set; }
		[DataMember(Name = "travelDuration")]
		public long TravelDuration { get; set; }
		[DataMember(Name = "maneuverPoint")]
		public Point ManeuverPoint { get; set; }
		[DataMember(Name = "instruction")]
		public Instruction Instruction { get; set; }
		[DataMember(Name = "compassDirection")]
		public string CompassDirection { get; set; }
		[DataMember(Name = "hint")]
		public Hint[] Hint { get; set; }
		[DataMember(Name = "warning")]
		public Warning[] Warning { get; set; }
	}
	[DataContract]
	public class Line
	{
		[DataMember(Name = "point")]
		public Point[] Point { get; set; }
	}
	[DataContract]
	public class Link
	{
		[DataMember(Name = "role")]
		public string Role { get; set; }
		[DataMember(Name = "name")]
		public string Name { get; set; }
		[DataMember(Name = "value")]
		public string Value { get; set; }
	}
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	public class Location : Resource
	{
		[DataMember(Name = "entityType")]
		public string EntityType { get; set; }
		[DataMember(Name = "address")]
		public Address Address { get; set; }
		[DataMember(Name = "confidence")]
		public string Confidence { get; set; }
	}
	[DataContract]
	public class Point : Shape
	{
		/// <summary>
		/// Latitude,Longitude
		/// </summary>
		[DataMember(Name = "coordinates")]
		public double[] Coordinates { get; set; }
		//[DataMember(Name = "latitude")]
		//public double Latitude { get; set; }
		//[DataMember(Name = "longitude")]
		//public double Longitude { get; set; }
	}
	[DataContract]
	[KnownType(typeof(Location))]
	[KnownType(typeof(Route))]
	public class Resource
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }
		[DataMember(Name = "id")]
		public string Id { get; set; }
		[DataMember(Name = "link")]
		public Link[] Link { get; set; }
		[DataMember(Name = "point")]
		public Point Point { get; set; }
		[DataMember(Name = "boundingBox")]
		public BoundingBox BoundingBox { get; set; }
	}
	[DataContract]
	public class ResourceSet
	{
		[DataMember(Name = "estimatedTotal")]
		public long EstimatedTotal { get; set; }
		[DataMember(Name = "resources")]
		public Resource[] Resources { get; set; }
	}
	[DataContract]
	public class Response
	{
		[DataMember(Name = "copyright")]
		public string Copyright { get; set; }
		[DataMember(Name = "brandLogoUri")]
		public string BrandLogoUri { get; set; }
		[DataMember(Name = "statusCode")]
		public int StatusCode { get; set; }
		[DataMember(Name = "statusDescription")]
		public string StatusDescription { get; set; }
		[DataMember(Name = "authenticationResultCode")]
		public string AuthenticationResultCode { get; set; }
		[DataMember(Name = "errorDetails")]
		public string[] errorDetails { get; set; }
		[DataMember(Name = "traceId")]
		public string TraceId { get; set; }
		[DataMember(Name = "resourceSets")]
		public ResourceSet[] ResourceSets { get; set; }
	}
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	public class Route : Resource
	{
		[DataMember(Name = "distanceUnit")]
		public string DistanceUnit { get; set; }
		[DataMember(Name = "durationUnit")]
		public string DurationUnit { get; set; }
		[DataMember(Name = "travelDistance")]
		public double TravelDistance { get; set; }
		[DataMember(Name = "travelDuration")]
		public long TravelDuration { get; set; }
		[DataMember(Name = "routeLegs")]
		public RouteLeg[] RouteLegs { get; set; }
		[DataMember(Name = "routePath")]
		public RoutePath RoutePath { get; set; }
	}
	[DataContract]
	public class RouteLeg
	{
		[DataMember(Name = "travelDistance")]
		public double TravelDistance { get; set; }
		[DataMember(Name = "travelDuration")]
		public long TravelDuration { get; set; }
		[DataMember(Name = "actualStart")]
		public Point ActualStart { get; set; }
		[DataMember(Name = "actualEnd")]
		public Point ActualEnd { get; set; }
		[DataMember(Name = "startLocation")]
		public Location StartLocation { get; set; }
		[DataMember(Name = "endLocation")]
		public Location EndLocation { get; set; }
		[DataMember(Name = "itineraryItems")]
		public ItineraryItem[] ItineraryItems { get; set; }
	}
	[DataContract]
	public class RoutePath
	{
		[DataMember(Name = "line")]
		public Line Line { get; set; }
	}
	[DataContract]
	[KnownType(typeof(Point))]
	public class Shape
	{
		[DataMember(Name = "boundingBox")]
		public double[] BoundingBox { get; set; }
	}
	[DataContract]
	public class Warning
	{
		[DataMember(Name = "warningType")]
		public string WarningType { get; set; }
		[DataMember(Name = "severity")]
		public string Severity { get; set; }
		[DataMember(Name = "value")]
		public string Value { get; set; }
	}
}