using System.Configuration;
using GeoCoding.Yahoo;

namespace GeoCoding.Tests
{
	public class YahooGeoCoderTest : GeoCoderTest
	{
		protected override IGeoCoder CreateGeoCoder()
		{
			return new YahooGeoCoder(ConfigurationManager.AppSettings["yahooAppId"]);
		}
	}
}