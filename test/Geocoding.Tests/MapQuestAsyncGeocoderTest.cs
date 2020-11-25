using Geocoding.MapQuest;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class MapQuestAsyncGeocoderTest : AsyncGeocoderTest
	{
		protected override IGeocoder CreateAsyncGeocoder()
		{
			return new MapQuestGeocoder(settings.MapQuestKey)
			{
				UseOSM = false
			};
		}
	}
}
