using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geocoding.MapQuest;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class MapQuestGeocoderTest : GeocoderTest
	{
		MapQuestGeocoder geocoder;

		public MapQuestGeocoderTest(SettingsFixture settings)
			: base(settings) { }

		protected override IGeocoder CreateGeocoder()
		{
			geocoder = new MapQuestGeocoder(settings.MapQuestKey)
			{
				UseOSM = false
			};
			return geocoder;
		}

		[Fact]
		public virtual async Task CanGeocodeNeighborhood()
		{
			// Regression test: Addresses with Quality=NEIGHBORHOOD are not returned
			Address[] addresses = (await geocoder.GeocodeAsync("North Sydney, New South Wales, Australia")).ToArray();
			Assert.NotEmpty(addresses);
		}

	}
}
