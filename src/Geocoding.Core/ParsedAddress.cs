namespace Geocoding
{
	/// <summary>
	/// Generic parsed address with each field separated out form the original FormattedAddress
	/// </summary>
	public class ParsedAddress : Address
	{
		public virtual string Street { get; set; }
		public virtual string City { get; set; }
		public virtual string County { get; set; }
		public virtual string State { get; set; }
		public virtual string Country { get; set; }
		public virtual string PostCode { get; set; }

		public ParsedAddress(string formattedAddress, Location coordinates, string provider)
			: base(formattedAddress, coordinates, provider) { }
	}
}
