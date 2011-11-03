using System;
using System.Configuration;
using GeoCoding.Google;

namespace GeoCoding.Tests
{
	public class GoogleGeoCoderTest : GeoCoderTest
	{
		protected override IGeoCoder CreateGeoCoder()
		{
			return new GoogleGeoCoder();
		}
	}
}