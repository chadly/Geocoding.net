namespace Geocoding.UsCensusBureau
{
	public class UsCensusBureauConstants
	{
		public const string Provider = "UsCensusBureau";

		public const string BaseUrl = "https://geocoding.geo.census.gov/geocoder/";
		public const string OneLineAddressPath = "locations/onelineaddress?";
		public const string AddressPath = "locations/address?";
		
		public const string AddressMatchesKey = "addressMatches";
		public const string MatchedAddressKey = "matchedAddress";
		public const string CoordinatesKey = "coordinates";
		public const string ResultKey = "result";
		public const string ErrorsKey = "errors";
		public const string XKey = "x";
		public const string YKey = "y";
	}
}
