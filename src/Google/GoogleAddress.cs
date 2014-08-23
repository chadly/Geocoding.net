using System;
using System.Linq;

namespace Geocoding.Google
{
	public class GoogleAddress : Address
	{
		readonly GoogleAddressType type;
		readonly GoogleAddressComponent[] components;
		readonly bool isPartialMatch;
		readonly GoogleViewport viewport;
      readonly GoogleBounds bounds;

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

		public GoogleViewport Viewport
		{
			get { return viewport; }
		}

      public GoogleBounds Bounds
      {
         get { return bounds; }
      }

		public GoogleAddressComponent this[GoogleAddressType type]
		{
			get { return Components.FirstOrDefault(c => c.Types.Contains(type)); }
		}

		public GoogleAddress(GoogleAddressType type, string formattedAddress, GoogleAddressComponent[] components, Location coordinates, GoogleViewport viewport, GoogleBounds bounds, bool isPartialMatch)
			: base(formattedAddress, coordinates, "Google")
		{
			if (components == null)
				throw new ArgumentNullException("components");

			if (components.Length < 1)
				throw new ArgumentException("Value cannot be empty.", "components");

			this.type = type;
			this.components = components;
			this.isPartialMatch = isPartialMatch;
			this.viewport = viewport;
		   this.bounds = bounds;
		}
	}
}