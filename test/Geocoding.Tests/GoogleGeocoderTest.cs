using System;
using System.Linq;
using Geocoding.Google;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class GoogleGeocoderTest : GeocoderTest
	{
		readonly SettingsFixture settings;
		GoogleGeocoder geocoder;

		public GoogleGeocoderTest(SettingsFixture settings)
		{
			this.settings = settings;
		}

		protected override IGeocoder CreateGeocoder()
		{
			string apiKey = settings.GoogleApiKey;

			if (String.IsNullOrEmpty(apiKey))
			{
				geocoder = new GoogleGeocoder();
			}
			else
			{
				geocoder = new GoogleGeocoder(apiKey);
			}

			return geocoder;
		}

		[Theory]
		[InlineData("United States", GoogleAddressType.Country)]
		[InlineData("Illinois, US", GoogleAddressType.AdministrativeAreaLevel1)]
		[InlineData("New York, New York", GoogleAddressType.Locality)]
		[InlineData("90210, US", GoogleAddressType.PostalCode)]
		[InlineData("1600 pennsylvania ave washington dc", GoogleAddressType.StreetAddress)]
        [InlineData("muswellbrook 2 New South Wales Australia", GoogleAddressType.Unknown)]
		public void CanParseAddressTypes(string address, GoogleAddressType type)
		{
			GoogleAddress[] addresses = geocoder.Geocode(address).ToArray();
			Assert.Equal(type, addresses[0].Type);
		}

        [Theory]
        [InlineData("United States", GoogleLocationType.Approximate)]
        [InlineData("Illinois, US", GoogleLocationType.Approximate)]
        [InlineData("Ingalls Corners Road, Canastota, NY 13032, USA", GoogleLocationType.GeometricCenter)]
        [InlineData("51 Harry S. Truman Parkway, Annapolis, MD 21401, USA", GoogleLocationType.RangeInterpolated)]
        [InlineData("1600 pennsylvania ave washington dc", GoogleLocationType.Rooftop)]
        [InlineData("muswellbrook 2 New South Wales Australia", GoogleLocationType.Approximate)]
        public void CanParseLocationTypes(string address, GoogleLocationType type)
        {
            GoogleAddress[] addresses = geocoder.Geocode(address).ToArray();
            Assert.Equal(type, addresses[0].LocationType);
        }

		[Theory]
		[InlineData("United States", "fr", "États-Unis")]
		[InlineData("Montreal", "en", "Montreal, QC, Canada")]
		[InlineData("Montreal", "fr", "Montréal, QC, Canada")]
		[InlineData("Montreal", "de", "Montreal, Québec, Kanada")]
		public void ApplyLanguage(string address, string language, string result)
		{
			geocoder.Language = language;
			GoogleAddress[] addresses = geocoder.Geocode(address).ToArray();
			Assert.Equal(result, addresses[0].FormattedAddress);
		}

		[Theory]
		[InlineData("Toledo", "us", "Toledo, OH, USA", null)]
		[InlineData("Toledo", "es", "Toledo, Spain", "Toledo, Toledo, Spain")]
		public void ApplyRegionBias(string address, string regionBias, string result1, string result2)
		{
			geocoder.RegionBias = regionBias;
			GoogleAddress[] addresses = geocoder.Geocode(address).ToArray();
			Assert.True(result1 == addresses[0].FormattedAddress || result2 == addresses[0].FormattedAddress);
		}

		[Theory]
		[InlineData("Winnetka", 46, -90, 47, -91, "Winnetka, IL, USA")]
		[InlineData("Winnetka", 34.172684, -118.604794, 34.236144, -118.500938, "Winnetka, Los Angeles, CA, USA")]
		public void ApplyBoundsBias(string address, double biasLatitude1, double biasLongitude1, double biasLatitude2, double biasLongitude2, string result)
		{
			geocoder.BoundsBias = new Bounds(biasLatitude1, biasLongitude1, biasLatitude2, biasLongitude2);
			GoogleAddress[] addresses = geocoder.Geocode(address).ToArray();
			Assert.Equal(result, addresses[0].FormattedAddress);
		}
	}
}