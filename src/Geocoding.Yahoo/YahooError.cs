namespace Geocoding.Yahoo
{
	/// <remarks>http://developer.yahoo.com/geo/placefinder/guide/responses.html#error-codes</remarks>
	public enum YahooError
	{
		NoError = 0,
		FeatureNotSupported = 1,
		NoInputParameters = 100,
		AddressNotUtf8 = 102,
		InsufficientAddressData = 103,
		UnknownLanguage = 104,
		NoCountryDetected = 105,
		CountryNotSupported = 106,
		UnknownError = 1000
	}
}