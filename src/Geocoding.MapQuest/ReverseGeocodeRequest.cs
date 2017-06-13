using System;
using Newtonsoft.Json;

namespace Geocoding.MapQuest
{
	public class ReverseGeocodeRequest : BaseRequest
	{
		public ReverseGeocodeRequest(string key, double latitude, double longitude)
			: this(key, new Location(latitude, longitude)) { }

		public ReverseGeocodeRequest(string key, Location loc)
			: this(key, new LocationRequest(loc)) { }

		public ReverseGeocodeRequest(string key, LocationRequest loc)
			: base(key)
		{
			Location = loc;
		}

		[JsonIgnore]
		LocationRequest loc;
		/// <summary>
		/// Latitude and longitude for the request
		/// </summary>
		[JsonProperty("location")]
		public virtual LocationRequest Location
		{
			get { return loc; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("Location");

				loc = value;
			}
		}

		[JsonIgnore]
		public override string RequestAction
		{
			get { return "reverse"; }
		}
	}
}
