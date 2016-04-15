using System.Collections.Generic;
using Geocoding.MapQuest;
using Xunit;

using Microsoft.Framework.Configuration;
namespace Geocoding.Tests
{
    public class MapQuestGeocoderTest : GeocoderTest
	{
		protected override IGeocoder CreateGeocoder()
		{
			var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			string k = config["AppSettings:mapQuestKey"];
			return new MapQuestGeocoder(k);
		}

		[Theory]
		[InlineData("Wilshire & Bundy, Los Angeles")]
		[InlineData("Fried St & 2nd St, Gretna, LA 70053")]
		public override void CanGeocodeWithSpecialCharacters(string address)
		{
			base.CanGeocodeWithSpecialCharacters(address);
		}
	}
}