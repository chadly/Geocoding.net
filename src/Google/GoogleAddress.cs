using System;

namespace Geocoding.Google
{
	public class GoogleAddress : Address
	{
		readonly GoogleAddressType type;
		readonly GoogleAddressComponent[] components;
		readonly bool isPartialMatch;

		public GoogleAddressType Type
		{
			get { return type; }
		}

		public GoogleAddressComponent[] Components
		{
			get { return components; }
		}

		public bool IsPartialMatch
		{
			get { return isPartialMatch; }
		}

		public GoogleAddress(GoogleAddressType type, string formattedAddress, GoogleAddressComponent[] components, Location coordinates, bool isPartialMatch)
			: base(formattedAddress, coordinates, "Google")
		{
			if (components == null)
				throw new ArgumentNullException("components");

			if (components.Length < 1)
				throw new ArgumentException("Value cannot be empty.", "components");

			this.type = type;
			this.components = components;
			this.isPartialMatch = isPartialMatch;
		}
	}
}