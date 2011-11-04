using System;
using System.Linq;
using System.Globalization;
using System.Threading;
using Xunit;
using Xunit.Extensions;

namespace GeoCoding.Tests
{
	public abstract class GeoCoderTest
	{
		readonly IGeoCoder geoCoder;

		public GeoCoderTest()
		{
			geoCoder = CreateGeoCoder();
		}

		protected abstract IGeoCoder CreateGeoCoder();

		[Fact]
		public void CanGeoCodeAddress()
		{
			Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave washington dc").ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Fact]
		public void CanGeoCodeNormalizedAddress()
		{
			Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave", "washington", "dc", null, null).ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanGeoCodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = geoCoder.GeoCode("24 sussex drive ottawa, ontario").ToArray();
			addresses[0].AssertCanadianPrimeMinister();
		}

		[Fact]
		public void ShouldNotBlowUpOnBadAddress()
		{
			var addresses = geoCoder.GeoCode("sdlkf;jasl;kjfldksjfasldf");
			Assert.Empty(addresses);
		}
	}
}