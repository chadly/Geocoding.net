using System;

namespace Geocoding.Google
{
	public class GoogleAddressComponent
	{
		public GoogleAddressType[] Types { get; private set; }
		public string LongName { get; private set; }
		public string ShortName { get; private set; }

		public GoogleAddressComponent(GoogleAddressType[] types, string longName, string shortName)
		{
			if (types == null)
				throw new ArgumentNullException("types");


			this.Types = types;
			this.LongName = longName;
			this.ShortName = shortName;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", Types[0], LongName);
		}
	}
}
