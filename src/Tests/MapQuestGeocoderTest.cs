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

		[Fact(Skip="Will not work for MapQuest None OSM")]
		public override void ShouldNotBlowUpOnBadAddress()
		{
			base.ShouldNotBlowUpOnBadAddress();
		}
	}
}
