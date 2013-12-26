using System.Configuration;
using System.Linq;
using GeoCoding.Google;
using Xunit;
using Xunit.Extensions;

namespace GeoCoding.Tests
{
	public class GoogleGeoCoderTest : GeoCoderTest
	{
		GoogleGeoCoder geoCoder;

		protected override IGeoCoder CreateGeoCoder()
		{
			geoCoder = new GoogleGeoCoder
			{
				ApiKey = ConfigurationManager.AppSettings["googleApiKey"]
			};
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

		[Theory]
		[InlineData("United States", "fr", "États-Unis")]
		[InlineData("Montreal", "en", "Montreal, QC, Canada")]
		[InlineData("Montreal", "fr", "Montréal, QC, Canada")]
		[InlineData("Montreal", "de", "Montreal, Québec, Kanada")]
		public void ApplyLanguage(string address, string language, string result)
		{
			geoCoder.Language = language;
			GoogleAddress[] addresses = geoCoder.GeoCode(address).ToArray();
			Assert.Equal(result, addresses[0].FormattedAddress);
		}

		[Theory]
		[InlineData("Toledo", "us", "Toledo, OH, USA")]
		[InlineData("Toledo", "es", "Toledo, Spain")]
		public void ApplyRegionBias(string address, string regionBias, string result)
		{
			geoCoder.RegionBias = regionBias;
			GoogleAddress[] addresses = geoCoder.GeoCode(address).ToArray();
			Assert.Equal(result, addresses[0].FormattedAddress);
		}

		[Theory]
		[InlineData("Winnetka", 46, -90, 47, -91, "Winnetka, IL, USA")]
		[InlineData("Winnetka", 34.172684, -118.604794, 34.236144, -118.500938, "Winnetka, Los Angeles, CA, USA")]
		public void ApplyBoundsBias(string address, double biasLatitude1, double biasLongitude1, double biasLatitude2, double biasLongitude2, string result)
		{
			geoCoder.BoundsBias = new Bounds(biasLatitude1, biasLongitude1, biasLatitude2, biasLongitude2);
			GoogleAddress[] addresses = geoCoder.GeoCode(address).ToArray();
			Assert.Equal(result, addresses[0].FormattedAddress);
		}
	}
}