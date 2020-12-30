using System;
using System.Linq;
using System.Threading.Tasks;
using Geocoding.UsCensusBureau;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class UsCensusBureauTest : GeocoderTest
	{
		public UsCensusBureauTest(SettingsFixture settings) : base(settings)
		{
		}

		protected override IGeocoder CreateGeocoder()
		{
			return new UsCensusBureauGeocoder();
		}

		[Theory(Skip = "not supported - us addresses only")]
		public override Task CanGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			return Task.CompletedTask;
		}

		[Theory(Skip = "not supported - reverse geocode")]
		public override Task CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			return Task.CompletedTask;
		}

		[Theory(Skip = "using different input with CanGeocodeWithSpecialCharacters2")]
		public override Task CanGeocodeWithSpecialCharacters(string address)
		{
			return Task.CompletedTask;
		}
		
		[Theory]
		[InlineData("12110 CLAYTON ROAD TOWN & COUNTRY,SAINT LOUIS,MO,63131,US")]
		public async Task CanGeocodeWithSpecialCharacters2(string address)
		{
			Address[] addresses = (await geocoder.GeocodeAsync(address)).ToArray();

			//asserting no exceptions are thrown and that we get something
			Assert.NotEmpty(addresses);
		}

		[Theory(Skip = "not supported - exact addresses only")]
		public override Task CanHandleStreetIntersectionsByAmpersand(string address)
		{
			return Task.CompletedTask;
		}

		[Fact(Skip = "not supported - reverse geocode")]
		public override Task CanReverseGeocodeAsync()
		{
			return Task.CompletedTask;
		}

		[Theory(Skip = "not supported")]
		public override Task CanGeocodeInvalidZipCodes(string address)
		{
			return Task.CompletedTask;
		}
	}
}
