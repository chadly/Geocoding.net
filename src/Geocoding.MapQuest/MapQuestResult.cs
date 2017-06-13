using System.Collections.Generic;
using Newtonsoft.Json;

namespace Geocoding.MapQuest
{
	/// <summary>
	/// Result obj returned in a collection of OSM response under the property: results
	/// </summary>
	public class MapQuestResult
	{
		[JsonProperty("locations")]
		public IList<MapQuestLocation> Locations { get; set; }

		[JsonProperty("providedLocation")]
		public MapQuestLocation ProvidedLocation { get; set; }
	}
}
