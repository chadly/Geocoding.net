using System.Linq;
using System.Threading.Tasks;
using Geocoding.Microsoft;
using Xunit;

namespace Geocoding.Tests
{
	public class BingMapsAsyncTest : AsyncGeocoderTest
    {
		BingMapsGeocoder geoCoder;
        

        protected override IGeocoder CreateAsyncGeocoder()
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
		public async Task CanParseAddressTypes(string address, EntityType type)
		{
		    var result = await geoCoder.GeocodeAsync(address);
			BingAddress[] addresses = result.ToArray();
			Assert.Equal(type, addresses[0].Type);
		}
	}
}
