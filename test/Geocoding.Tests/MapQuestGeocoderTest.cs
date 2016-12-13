using Geocoding.MapQuest;
using Xunit;
namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class MapQuestGeocoderTest : GeocoderTest
	{
		readonly SettingsFixture settings;

		public MapQuestGeocoderTest(SettingsFixture settings)
		{
			this.settings = settings;
		}

		protected override IGeocoder CreateGeocoder()
		{
			return new MapQuestGeocoder(settings.MapQuestKey);
			return new MapQuestGeocoder(k) { UseOSM = false };
		}
	}
}
