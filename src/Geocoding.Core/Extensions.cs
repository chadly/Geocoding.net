using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Geocoding
{
	public static class Extensions
	{
		public static bool IsNullOrEmpty<T>(this ICollection<T> col)
		{
			return col == null || col.Count == 0;
		}

		public static void ForEach<T>(this IEnumerable<T> self, Action<T> actor)
		{
			if (actor == null)
				throw new ArgumentNullException("actor");

			if (self == null)
				return;

			foreach (T item in self)
			{
				actor(item);
			}
		}

		//Universal ISO DT Converter
		static readonly JsonConverter[] JSON_CONVERTERS = new JsonConverter[]
		{
			new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal },
			new StringEnumConverter(),
		};

		public static string ToJSON(this object o)
		{
			string result = null;
			if (o != null)
				result = JsonConvert.SerializeObject(o, Formatting.Indented, JSON_CONVERTERS);
			return result ?? string.Empty;
		}

		public static T FromJSON<T>(this string json)
		{
			T o = default(T);
			if (!string.IsNullOrWhiteSpace(json))
				o = JsonConvert.DeserializeObject<T>(json, JSON_CONVERTERS);
			return o;
		}
	}
}
