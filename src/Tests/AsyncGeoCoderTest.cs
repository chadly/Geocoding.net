using System.Globalization;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Extensions;

namespace GeoCoding.Tests
{
	public abstract class AsyncGeoCoderTest
	{
		readonly IAsyncGeoCoder asyncGeoCoder;

		public AsyncGeoCoderTest()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");

			asyncGeoCoder = CreateAsyncGeoCoder();
		}

		protected abstract IAsyncGeoCoder CreateAsyncGeoCoder();

		[Fact]
		public void CanGeoCodeAddress()
		{
			asyncGeoCoder.GeoCodeAsync("1600 pennsylvania ave washington dc").ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertWhiteHouse();
			});
		}

		[Fact]
		public void CanGeoCodeNormalizedAddress()
		{
			asyncGeoCoder.GeoCodeAsync("1600 pennsylvania ave", "washington", "dc", null, null).ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertWhiteHouse();
			});
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanGeoCodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			asyncGeoCoder.GeoCodeAsync("24 sussex drive ottawa, ontario").ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertCanadianPrimeMinister();
			});
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanReverseGeoCodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			asyncGeoCoder.ReverseGeocodeAsync(38.8976777, -77.036517).ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertWhiteHouseArea();
			});
		}

		[Fact]
		public void ShouldNotBlowUpOnBadAddress()
		{
			asyncGeoCoder.GeoCodeAsync("sdlkf;jasl;kjfldksjfasldf").ContinueWith(task =>
			{
				var addresses = task.Result;
				Assert.Empty(addresses);
			});
		}

		[Fact]
		public void CanGeoCodeWithSpecialCharacters()
		{
			asyncGeoCoder.GeoCodeAsync("Fried St & 2nd St, Gretna, LA 70053").ContinueWith(task =>
			{
				var addresses = task.Result;

				//asserting no exceptions are thrown and that we get something
				Assert.NotEmpty(addresses);
			});
		}

		[Fact]
		public void CanReverseGeoCode()
		{
			asyncGeoCoder.ReverseGeocodeAsync(38.8976777, -77.036517).ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertWhiteHouseArea();
			});
		}
	}
}
