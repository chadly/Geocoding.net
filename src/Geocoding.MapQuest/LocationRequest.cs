using System;
using Newtonsoft.Json;

namespace Geocoding.MapQuest
{
	public class LocationRequest
	{
		public LocationRequest(string street)
		{
			Street = street;
		}

		public LocationRequest(Location location)
		{
			Location = location;
		}

		[JsonIgnore]
		string street;
		/// <summary>
		/// Full street address or intersection for geocoding
		/// </summary>
		[JsonProperty("street", NullValueHandling = NullValueHandling.Ignore)]
		public virtual string Street
		{
			get { return street; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentException("Street can not be null or blank");

				street = value;
			}
		}

		[JsonIgnore]
		Location location;
		/// <summary>
		/// Latitude and longitude for reverse geocoding
		/// </summary>
		[JsonProperty("latLng", NullValueHandling = NullValueHandling.Ignore)]
		public virtual Location Location
		{
			get { return location; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("Location");

				location = value;
			}
		}

		public override string ToString()
		{
			return string.Format("street: {0}", Street);
		}

	}
}
