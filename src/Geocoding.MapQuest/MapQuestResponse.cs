using Newtonsoft.Json;
using System.Collections.Generic;

namespace Geocoding.MapQuest
{
	public class MapQuestResponse
	{
		//[JsonArray(AllowNullItems=true)]
		[JsonProperty("results")]
		public IList<MapQuestResult> Results { get; set; }

		[JsonProperty("options")]
		public RequestOptions Options { get; set; }

		[JsonProperty("info")]
		public ResponseInfo Info { get; set; }
	}
}