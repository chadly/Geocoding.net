using System;
using System.Globalization;
using System.Net;

namespace Geocoding
{
	public class QueryParameter
	{
		public String Name { get; }
		public String Value { get; }

		public QueryParameter(String name, String value)
		{
			Name = name;
			Value = value;
		}

		public String Query
		{
			get
			{
				return String.Format(CultureInfo.InvariantCulture,
					"{0}={1}",
					WebUtility.UrlEncode(Name),
					WebUtility.UrlEncode(Value));
			}
		}
	}
}
