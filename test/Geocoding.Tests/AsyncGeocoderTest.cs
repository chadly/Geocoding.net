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
			//Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");

			asyncGeocoder = CreateAsyncGeocoder();
		}

		protected abstract IGeocoder CreateAsyncGeocoder();

		[Fact]
		public void CanGeocodeAddress()
		{
			asyncGeocoder.GeocodeAsync("1600 pennsylvania ave washington dc").ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertWhiteHouse();
			});
		}

		[Fact]
		public void CanGeocodeNormalizedAddress()
		{
			asyncGeocoder.GeocodeAsync("1600 pennsylvania ave", "washington", "dc", null, null).ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertWhiteHouse();
			});
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public async Task CanGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			//Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

		    var result = await asyncGeocoder.GeocodeAsync("24 sussex drive ottawa, ontario");
			Address[] addresses = result.ToArray();
			addresses[0].AssertCanadianPrimeMinister();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public async Task CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			//Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

		    var result = await asyncGeocoder.ReverseGeocodeAsync(38.8976777, -77.036517);
			Address[] addresses = result.ToArray();
			addresses[0].AssertWhiteHouseArea();
		}

		[Fact]
		public void ShouldNotBlowUpOnBadAddress()
		{
			asyncGeocoder.GeocodeAsync("sdlkf;jasl;kjfldksjfasldf").ContinueWith(task =>
			{
				var addresses = task.Result;
				Assert.Empty(addresses);
			});
		}

		[Fact]
		public void CanGeocodeWithSpecialCharacters()
		{
			asyncGeocoder.GeocodeAsync("Fried St & 2nd St, Gretna, LA 70053").ContinueWith(task =>
			{
				var addresses = task.Result;

				//asserting no exceptions are thrown and that we get something
				Assert.NotEmpty(addresses);
			});
		}

		[Fact]
		public void CanReverseGeocodeAsync()
		{
			asyncGeocoder.ReverseGeocodeAsync(38.8976777, -77.036517).ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertWhiteHouseArea();
			});
		}
	}
}
