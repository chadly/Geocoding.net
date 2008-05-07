using System;
using System.Configuration;
using GeoCoding.Services.Yahoo;

namespace GeoCoding.Services.Tests
{
    public class YahooGeoCoderTest : GeoCoderTest
    {
        protected override IGeoCoder CreateGeoCoder()
        {
            return new YahooGeoCoder(ConfigurationManager.AppSettings["yahooAppId"]);
        }
    }
}
