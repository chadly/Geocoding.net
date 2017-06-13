using Geocoding.MapQuest;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class MapQuestBatchGeocoderTest : BatchGeocoderTest
	{
		readonly SettingsFixture settings;

		public MapQuestBatchGeocoderTest(SettingsFixture settings)
		{
			this.settings = settings;
		}

		protected override IBatchGeocoder CreateBatchGeocoder()
		{
			return new MapQuestGeocoder(settings.MapQuestKey);
		}
	}
}
