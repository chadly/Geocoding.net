using System;
using System.Configuration;
using GeoCoding.VirtualEarth;

namespace GeoCoding.Tests
{
	public class VirtualEarthGeoCoderTest : GeoCoderTest, IDisposable
	{
		private VirtualEarthGeoCoder geoCoder;

		protected override IGeoCoder CreateGeoCoder()
		{
			string username = ConfigurationManager.AppSettings["virtualEarthUsername"];
			string password = ConfigurationManager.AppSettings["virtualEarthPassword"];

			geoCoder = new VirtualEarthGeoCoder(username, password, true);
			return geoCoder;
		}

		public void Dispose()
		{
			if (geoCoder != null)
				geoCoder.Dispose();
		}
	}
}