using Geocoding.MapQuest;
using System.Configuration;
using Xunit.Extensions;

namespace Geocoding.Tests
{
	public class MapQuestGeocoderTest : GeocoderTest
	{
		protected override IGeocoder CreateGeocoder()
		{
			string k = ConfigurationManager.AppSettings["mapQuestKey"];
			return new MapQuestGeocoder(k) { UseOSM = false };
		}
	}
}
