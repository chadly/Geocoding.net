using System.Linq;
using GeoCoding.Google;
using Xunit;
using Xunit.Extensions;
using System.Net;

namespace GeoCoding.Tests
{
	public class GoogleGeoCoderTest : GeoCoderTest
	{
		GoogleGeoCoder geoCoder;

		protected override IGeoCoder CreateGeoCoder()
		{
            geoCoder = new GoogleGeoCoder();
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
			GoogleAddress[] addresses = geoCoder.GeoCode(address).ToArray();
			Assert.Equal(type, addresses[0].Type);
		}
	}
}