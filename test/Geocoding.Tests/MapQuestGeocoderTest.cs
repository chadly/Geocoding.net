using Geocoding.MapQuest;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class MapQuestGeocoderTest : GeocoderTest
	{
		public MapQuestGeocoderTest(SettingsFixture settings)
			: base(settings) { }

		protected override IGeocoder CreateGeocoder()
		{
			return new MapQuestGeocoder(settings.MapQuestKey)
			{
				UseOSM = false
			};
		}
	}
}
