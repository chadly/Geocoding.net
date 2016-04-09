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
		}

		[Theory]
		[InlineData("Wilshire & Bundy, Los Angeles")]
		[InlineData("Fried St & 2nd St, Gretna, LA 70053")]
		public override void CanGeocodeWithSpecialCharacters(string address)
		{
			base.CanGeocodeWithSpecialCharacters(address);
		}
	}
}
