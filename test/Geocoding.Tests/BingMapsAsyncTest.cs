using System.Linq;
using Geocoding.Microsoft;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class BingMapsAsyncTest : AsyncGeocoderTest
	{
		readonly SettingsFixture settings;
		BingMapsGeocoder geoCoder;

		public BingMapsAsyncTest(SettingsFixture settings)
		{
			this.settings = settings;
		}

		protected override IAsyncGeocoder CreateAsyncGeocoder()
		{
			geoCoder = new BingMapsGeocoder(settings.BingMapsKey);
			return geoCoder;
		}

		[Theory]
		[InlineData("United States", EntityType.CountryRegion)]
		[InlineData("Illinois, US", EntityType.AdminDivision1)]
		[InlineData("New York, New York", EntityType.PopulatedPlace)]
		[InlineData("90210, US", EntityType.Postcode1)]
		[InlineData("1600 pennsylvania ave washington dc", EntityType.Address)]
		public void CanParseAddressTypes(string address, EntityType type)
		{
			geoCoder.GeocodeAsync(address).ContinueWith(task =>
			{
				BingAddress[] addresses = task.Result.ToArray();
				Assert.Equal(type, addresses[0].Type);
			});
		}
	}
}
