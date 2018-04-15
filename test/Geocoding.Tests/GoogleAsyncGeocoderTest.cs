using System;
using System.Linq;
using System.Threading.Tasks;
using Geocoding.Google;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class GoogleAsyncGeocoderTest : AsyncGeocoderTest
	{
		GoogleGeocoder geoCoder;

		protected override IGeocoder CreateAsyncGeocoder()
		{
			string apiKey = settings.GoogleApiKey;

			if (String.IsNullOrEmpty(apiKey))
			{
				geoCoder = new GoogleGeocoder();
			}
			else
			{
				geoCoder = new GoogleGeocoder(apiKey);
			}

			return geoCoder;
		}

		[Theory]
		[InlineData("United States", GoogleAddressType.Country)]
		[InlineData("Illinois, US", GoogleAddressType.AdministrativeAreaLevel1)]
		[InlineData("New York, New York", GoogleAddressType.Locality)]
		[InlineData("90210, US", GoogleAddressType.PostalCode)]
		[InlineData("1600 pennsylvania ave washington dc", GoogleAddressType.Establishment)]
		public async Task CanParseAddressTypes(string address, GoogleAddressType type)
		{
		    var result = await geoCoder.GeocodeAsync(address);
			GoogleAddress[] addresses = result.ToArray();
			Assert.Equal(type, addresses[0].Type);
		}
	}
}
