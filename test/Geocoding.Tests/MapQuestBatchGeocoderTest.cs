using Microsoft.Framework.Configuration;

namespace Geocoding.Tests
{
    public class MapQuestBatchGeocoderTest : BatchGeocoderTest
	{
		protected override IBatchGeocoder CreateBatchGeocoder()
		{
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string k = config["AppSettings:mapQuestKey"];
			return new MapQuest.MapQuestGeocoder(k);
		}
	}
}
