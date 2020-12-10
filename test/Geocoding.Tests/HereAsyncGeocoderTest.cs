using Geocoding.Here;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class HereAsyncGeocoderTest : AsyncGeocoderTest
	{
		HereGeocoder geoCoder;

		protected override IGeocoder CreateAsyncGeocoder()
		{
			geoCoder = new HereGeocoder(settings.HereApiKey);
			return geoCoder;
		}
	}
}
