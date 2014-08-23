using System.Configuration;
using System.Linq;
using Geocoding.Google;
using Xunit;
using Xunit.Extensions;

namespace Geocoding.Tests
{
	public class GoogleAsyncGeocoderTest : AsyncGeocoderTest
	{
		GoogleGeocoder geoCoder;

		protected override IAsyncGeocoder CreateAsyncGeocoder()
		{
			geoCoder = new GoogleGeocoder(ConfigurationManager.AppSettings["googleApiKey"]);
			return geoCoder;
		}

		[Theory]
		[InlineData("United States", GoogleAddressType.Country)]
		[InlineData("Illinois, US", GoogleAddressType.AdministrativeAreaLevel1)]
		[InlineData("New York, New York", GoogleAddressType.Locality)]
		[InlineData("90210, US", GoogleAddressType.PostalCode)]
		[InlineData("1600 pennsylvania ave washington dc", GoogleAddressType.StreetAddress)]
		public void CanParseAddressTypes(string address, GoogleAddressType type)
		{
			geoCoder.GeocodeAsync(address).ContinueWith(task =>
			{
				GoogleAddress[] addresses = task.Result.ToArray();
				Assert.Equal(type, addresses[0].Type);
			});
		}
	}
}
