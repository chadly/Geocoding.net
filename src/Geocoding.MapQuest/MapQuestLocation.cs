using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Geocoding.MapQuest
{
	/// <summary>
	/// MapQuest address obj.
	/// <see cref="http://open.mapquestapi.com/geocoding/"/>
	/// </summary>
	public class MapQuestLocation : ParsedAddress
	{
		const string UNKNOWN = "unknown";
		static readonly string DEFAULT_LOC = new Location(0, 0).ToString();

		public MapQuestLocation(string formattedAddress, Location coordinates)
			: base(
				string.IsNullOrWhiteSpace(formattedAddress) ? UNKNOWN : formattedAddress,
				coordinates ?? new Location(0, 0),
				"MapQuest")
		{
			DisplayCoordinates = coordinates;
		}

		[JsonProperty("location")]
		public override string FormattedAddress
		{
			get
			{
				return ToString();
			}
			set { base.FormattedAddress = value; }
		}

		[JsonProperty("latLng")]
		public override Location Coordinates
		{
			get { return base.Coordinates; }
			set { base.Coordinates = value; }
		}

		[JsonProperty("displayLatLng")]
		public virtual Location DisplayCoordinates { get; set; }

		[JsonProperty("street")]
		public override string Street { get; set; }

		[JsonProperty("adminArea5")]
		public override string City { get; set; }

		[JsonProperty("adminArea4")]
		public override string County { get; set; }

		[JsonProperty("adminArea3")]
		public override string State { get; set; }

		[JsonProperty("adminArea1")]
		public override string Country { get; set; }

		[JsonProperty("postalCode")]
		public override string PostCode { get; set; }

		public override string ToString()
		{
			if (base.FormattedAddress != UNKNOWN)
				return base.FormattedAddress;
			else
			{
				var sb = new StringBuilder();
				if (!string.IsNullOrWhiteSpace(Street))
					sb.AppendFormat("{0}, ", Street);

				if (!string.IsNullOrWhiteSpace(City))
					sb.AppendFormat("{0}, ", City);

				if (!string.IsNullOrWhiteSpace(State))
					sb.AppendFormat("{0} ", State);
				else if (!string.IsNullOrWhiteSpace(County))
					sb.AppendFormat("{0} ", County);

				if (!string.IsNullOrWhiteSpace(PostCode))
					sb.AppendFormat("{0} ", PostCode);

				if (!string.IsNullOrWhiteSpace(Country))
					sb.AppendFormat("{0} ", Country);

				if (sb.Length > 1)
				{
					sb.Length--;

					string s = sb.ToString();
					if (s.Last() == ',')
						s = s.Remove(s.Length - 1);

					return s;
				}
				else if (Coordinates != null && Coordinates.ToString() != DEFAULT_LOC)
					return Coordinates.ToString();
				else
					return UNKNOWN;
			}
		}

		/// <summary>
		/// Type of location
		/// </summary>
		[JsonProperty("type")]
		public virtual LocationType Type { get; set; }

		/// <summary>
		/// Granularity code of quality/accuracy guarantee
		/// <see cref="http://open.mapquestapi.com/geocoding/geocodequality.html#granularity"/>
		/// </summary>
		[JsonProperty("geocodeQuality")]
		public virtual Quality Quality { get; set; }

		/// <summary>
		/// Text string comparable, sort able score
		/// <see cref="http://open.mapquestapi.com/geocoding/geocodequality.html#granularity"/>
		/// </summary>
		[JsonProperty("geocodeQualityCode")]
		public virtual string Confidence { get; set; }

		/// <summary>
		/// Identifies the closest road to the address for routing purposes.
		/// </summary>
		[JsonProperty("linkId")]
		public virtual string LinkId { get; set; }

		/// <summary>
		/// Which side of the street this address is in
		/// </summary>
		[JsonProperty("sideOfStreet")]
		public virtual SideOfStreet SideOfStreet { get; set; }

		/// <summary>
		/// Url to a MapQuest map
		/// </summary>
		[JsonProperty("mapUrl")]
		public virtual Uri MapUrl { get; set; }

		[JsonProperty("adminArea1Type")]
		public virtual string CountryLabel { get; set; }

		[JsonProperty("adminArea3Type")]
		public virtual string StateLabel { get; set; }
	}
}