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
	public class MapQuestBatchGeocoderTest : BatchGeocoderTest
	{
		protected override IBatchGeocoder CreateBatchGeocoder()
		{
			string k = ConfigurationManager.AppSettings["mapQuestKey"];
			return new MapQuest.MapQuestGeocoder(k);
		}
	}
}
