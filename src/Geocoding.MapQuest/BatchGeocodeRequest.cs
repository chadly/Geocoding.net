using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Geocoding.MapQuest
{
	public class BatchGeocodeRequest : BaseRequest
	{
		public BatchGeocodeRequest(string key, ICollection<string> addresses)
			: base(key)
		{
			if (addresses.IsNullOrEmpty())
				throw new ArgumentException("addresses can not be null or empty");

			Locations = (from l in addresses select new LocationRequest(l)).ToArray();
		}

		[JsonIgnore]
		readonly List<LocationRequest> _locations = new List<LocationRequest>();
		/// <summary>
		/// Required collection of concatenated address string
		/// Note input will be hashed for uniqueness.
		/// Order is not guaranteed.
		/// </summary>
		[JsonProperty("locations")]
		public ICollection<LocationRequest> Locations
		{
			get { return _locations; }
			set
			{
				if (value.IsNullOrEmpty())
					throw new ArgumentNullException("Locations can not be null or empty!");

				_locations.Clear();
				(from v in value
				 where v != null
				 select v).ForEach(v => _locations.Add(v));

				if (_locations.Count == 0)
					throw new InvalidOperationException("At least one valid Location is required");
			}
		}

		public override string RequestAction
		{
			get { return "batch"; }
		}
	}
}
