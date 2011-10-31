using System;
using System.Configuration;
using GeoCoding.BingMaps;

namespace GeoCoding.Tests
{
	public class BingMapsTest : GeoCoderTest
	{
		protected override IGeoCoder CreateGeoCoder()
		{
			return new BingMapsGeoCoder(ConfigurationManager.AppSettings["bingMapsKey"]);
		}
	}
}
