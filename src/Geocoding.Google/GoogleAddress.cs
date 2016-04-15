using System;
using System.Linq;

namespace Geocoding.Google
{
    public class GoogleAddress : Address
    {
        private readonly GoogleAddressType type;
        private readonly GoogleLocationType locationType;
        private readonly GoogleAddressComponent[] components;
        private readonly bool isPartialMatch;
        private readonly GoogleViewport viewport;

        public GoogleAddressType Type
        {
            get { return type; }
        }

        public GoogleLocationType LocationType
        {
            get { return locationType; }
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

        public GoogleAddressComponent this[GoogleAddressType type]
        {
            get { return Components.FirstOrDefault(c => c.Types.Contains(type)); }
        }

        public GoogleAddress(GoogleAddressType type, string formattedAddress, GoogleAddressComponent[] components,
            Location coordinates, GoogleViewport viewport, bool isPartialMatch, GoogleLocationType locationType)
            : base(formattedAddress, coordinates, "Google")
        {
            if (components == null)
                throw new ArgumentNullException("components");

            this.type = type;
            this.components = components;
            this.isPartialMatch = isPartialMatch;
            this.viewport = viewport;
            this.locationType = locationType;
        }
    }
}