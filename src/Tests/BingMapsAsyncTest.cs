using System.Configuration;
using System.Linq;
using GeoCoding.Microsoft;
using Xunit;
using Xunit.Extensions;

namespace GeoCoding.Tests
{
	public class BingMapsAsyncTest : AsyncGeoCoderTest
	{
		BingMapsGeoCoder geoCoder;

		protected override IAsyncGeoCoder CreateAsyncGeoCoder()
		{
			geoCoder = new BingMapsGeoCoder(ConfigurationManager.AppSettings["bingMapsKey"]);
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
			geoCoder.GeoCodeAsync(address).ContinueWith(task =>
			{
				BingAddress[] addresses = task.Result.ToArray();
				Assert.Equal(type, addresses[0].Type);
			});
		}
	}
}
