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
			return new HereGeocoder(settings.HereAppId, settings.HereAppCode);
		}
	}
}
