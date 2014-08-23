using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Xunit;
using Xunit.Extensions;

using Geocoding.MapQuest;

namespace Geocoding.Tests
{
	public class MapQuestGeocoderTest : GeocoderTest
	{
		protected override IGeocoder CreateGeocoder()
		{
			string k = ConfigurationManager.AppSettings["mapQuestKey"];
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
