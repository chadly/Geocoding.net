using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Geocoding.Tests
{
	public abstract class AsyncGeocoderTest
	{
		readonly IGeocoder asyncGeocoder;
        protected readonly SettingsFixture settings = new SettingsFixture();

        public AsyncGeocoderTest()
		{
			CultureInfo.CurrentCulture = new CultureInfo("en-us");

			asyncGeocoder = CreateAsyncGeocoder();
		}

		protected abstract IGeocoder CreateAsyncGeocoder();

		[Fact]
		public async Task CanGeocodeAddress()
		{
			var addresses = await asyncGeocoder.GeocodeAsync("1600 pennsylvania ave washington dc");
			addresses.First().AssertWhiteHouse();
		}

		[Fact]
		public async Task CanGeocodeNormalizedAddress()
		{
			var addresses = await asyncGeocoder.GeocodeAsync("1600 pennsylvania ave", "washington", "dc", null, null);
			addresses.First().AssertWhiteHouse();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public async Task CanGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			CultureInfo.CurrentCulture = new CultureInfo(cultureName);

		    var addresses = await asyncGeocoder.GeocodeAsync("24 sussex drive ottawa, ontario");
			addresses.First().AssertCanadianPrimeMinister();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public async Task CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			CultureInfo.CurrentCulture = new CultureInfo(cultureName);

		    var addresses = await asyncGeocoder.ReverseGeocodeAsync(38.8976777, -77.036517);
			addresses.First().AssertWhiteHouseArea();
		}

		[Fact]
		public async Task ShouldNotBlowUpOnBadAddress()
		{
			var addresses = await asyncGeocoder.GeocodeAsync("sdlkf;jasl;kjfldksjfasldf");
			Assert.Empty(addresses);
		}

		[Fact]
		public async Task CanGeocodeWithSpecialCharacters()
		{
			var addresses = await asyncGeocoder.GeocodeAsync("Fried St & 2nd St, Gretna, LA 70053");
			Assert.NotEmpty(addresses);
		}

		[Fact]
		public async Task CanReverseGeocodeAsync()
		{
			var addresses = await asyncGeocoder.ReverseGeocodeAsync(38.8976777, -77.036517);
			addresses.First().AssertWhiteHouse();
		}
	}
}
