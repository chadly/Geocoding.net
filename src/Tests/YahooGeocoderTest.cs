using System.Configuration;
using Geocoding.Yahoo;

namespace Geocoding.Tests
{
	public class YahooGeocoderTest : GeocoderTest
	{
		protected override IGeocoder CreateGeocoder()
		{
			return new YahooGeocoder(
				ConfigurationManager.AppSettings["yahooConsumerKey"],
				ConfigurationManager.AppSettings["yahooConsumerSecret"]
			);
		}
	}
}