using System;
using System.Configuration;
using GeoCoding.Services.Google;

namespace GeoCoding.Services.Tests
{
    public class GoogleGeoCoderTest : GeoCoderTest
    {
        protected override IGeoCoder CreateGeoCoder()
        {
            return new GoogleGeoCoder(ConfigurationManager.AppSettings["googleAccessKey"]);
        }
    }
}
