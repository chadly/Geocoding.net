using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Geocoding.Tests
{
	public abstract class GeocoderTest
	{
		readonly IGeocoder geocoder;
		protected readonly SettingsFixture settings;

		public GeocoderTest(SettingsFixture settings)
		{
			//Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");

			this.settings = settings;
			geocoder = CreateGeocoder();
		}

		protected abstract IGeocoder CreateGeocoder();

		[Theory]
		[InlineData("1600 pennsylvania ave nw, washington dc")]
		public virtual async Task CanGeocodeAddress(string address)
		{
			Address[] addresses = (await geocoder.GeocodeAsync(address)).ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Fact]
		public virtual async Task CanGeocodeNormalizedAddress()
		{
			Address[] addresses = (await geocoder.GeocodeAsync("1600 pennsylvania ave nw", "washington", "dc", null, null)).ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public virtual async Task CanGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			//Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = (await geocoder.GeocodeAsync("24 sussex drive ottawa, ontario")).ToArray();
			addresses[0].AssertCanadianPrimeMinister();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public virtual async Task CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			//Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = (await geocoder.ReverseGeocodeAsync(38.8976777, -77.036517)).ToArray();
			addresses[0].AssertWhiteHouseArea();
		}

		[Fact]
		public virtual async Task ShouldNotBlowUpOnBadAddress()
		{
			Address[] addresses = (await geocoder.GeocodeAsync("sdlkf;jasl;kjfldksj,fasldf")).ToArray();
			Assert.Empty(addresses);
		}

		[Theory]
		[InlineData("40 1/2 Road")]
		[InlineData("B's Farm RD")]
		[InlineData("Wilshire & Bundy Plaza, Los Angeles")]
		public virtual async Task CanGeocodeWithSpecialCharacters(string address)
		{
			Address[] addresses = (await geocoder.GeocodeAsync(address)).ToArray();

			//asserting no exceptions are thrown and that we get something
			Assert.NotEmpty(addresses);
		}

		[Theory]
		[InlineData("Wilshire & Centinela, Los Angeles")]
		[InlineData("Fried St & 2nd St, Gretna, LA 70053")]
		public virtual async Task CanHandleStreetIntersectionsByAmpersand(string address)
		{
			Address[] addresses = (await geocoder.GeocodeAsync(address)).ToArray();

			//asserting no exceptions are thrown and that we get something
			Assert.NotEmpty(addresses);
		}

		[Fact]
		public virtual async Task CanReverseGeocodeAsync()
		{
			Address[] addresses = (await geocoder.ReverseGeocodeAsync(38.8976777, -77.036517)).ToArray();
			addresses[0].AssertWhiteHouseArea();
		}

		[Theory]
		[InlineData("1 Robert Wood Johnson Hosp New Brunswick, NJ 08901 USA")]
		[InlineData("miss, MO")]
		//https://github.com/chadly/Geocoding.net/issues/6
		public virtual async Task CanGeocodeInvalidZipCodes(string address)
		{
			Address[] addresses = (await geocoder.GeocodeAsync(address)).ToArray();
			Assert.NotEmpty(addresses);
		}
	}
}