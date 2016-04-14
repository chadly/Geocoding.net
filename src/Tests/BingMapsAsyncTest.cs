﻿using System.Linq;
using Geocoding.Microsoft;
using Xunit;
using Microsoft.Framework.Configuration;

namespace Geocoding.Tests
{
    public class BingMapsAsyncTest : AsyncGeocoderTest
	{
		BingMapsGeocoder geoCoder;

		protected override IAsyncGeocoder CreateAsyncGeocoder()
		{
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            geoCoder = new BingMapsGeocoder(config["AppSettings:bingMapsKey"]);
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
