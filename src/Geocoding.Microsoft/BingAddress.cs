namespace Geocoding.Microsoft
{
	public class BingAddress : Address
	{
		private readonly string addressLine, adminDistrict, adminDistrict2, countryRegion, locality, postalCode;
		private readonly EntityType type;
		private readonly ConfidenceLevel confidence;

		public string AddressLine
		{
			get { return addressLine ?? ""; }
		}

		public string AdminDistrict
		{
			get { return adminDistrict ?? ""; }
		}

		public string AdminDistrict2
		{
			get { return adminDistrict2 ?? ""; }
		}

		public string CountryRegion
		{
			get { return countryRegion ?? ""; }
		}

		public string Locality
		{
			get { return locality ?? ""; }
		}

		public string PostalCode
		{
			get { return postalCode ?? ""; }
		}

		public EntityType Type
		{
			get { return type; }
		}

		public ConfidenceLevel Confidence
		{
			get { return confidence; }
		}

		public BingAddress(string formattedAddress, Location coordinates, string addressLine, string adminDistrict, string adminDistrict2,
			string countryRegion, string locality, string postalCode, EntityType type, ConfidenceLevel confidence)
			: base(formattedAddress, coordinates, "Bing")
		{
			this.addressLine = addressLine;
			this.adminDistrict = adminDistrict;
			this.adminDistrict2 = adminDistrict2;
			this.countryRegion = countryRegion;
			this.locality = locality;
			this.postalCode = postalCode;
			this.type = type;
			this.confidence = confidence;
		}
	}
}