using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace Geocoding
{
	/// <summary>
	/// Provides a custom constructor for Uri query strings.
	/// </summary>
	public class QueryBuilder
	{
		private readonly List<QueryParameter> parameters = new List<QueryParameter>();

		/// <summary>
		/// Adds the parameter.
		/// </summary>
		/// <param name="parameter">The parameter to add.</param>
		/// <returns>This instance of the Query builder, to allow Fluent calls.</returns>
		public QueryBuilder AddParameter(QueryParameter parameter)
		{
			parameters.Add(parameter);
			return this;
		}

		/// <summary>
		/// Adds the parameter.
		/// </summary>
		/// <param name="name">The name/key of the parameter.</param>
		/// <param name="value">The value of parameter.</param>
		/// <remarks>Both name and value will be Url Encoded.</remarks>
		/// <returns>This instance of the Query builder, to allow Fluent calls.</returns>
		public QueryBuilder AddParameter(String name, String value)
		{
			return AddParameter(new QueryParameter(name, value));
		}

		/// <summary>
		/// Adds the parameter if the value is not null or Empty.
		/// </summary>
		/// <param name="name">The name/key of the parameter.</param>
		/// <param name="value">The value of parameter.</param>
		/// <remarks>Both name and value will be Url Encoded.</remarks>
		/// <returns>This instance of the Query builder, to allow Fluent calls.</returns>
		public QueryBuilder AddNonEmptyParameter(String name, String value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				AddParameter(new QueryParameter(name, value));
			}
			return this;
		}

		/// <summary>
		/// Adds the parameter.
		/// </summary>
		/// <param name="parameters">The parameter to add.</param>
		/// <returns>This instance of the Query builder, to allow Fluent calls.</returns>
		public QueryBuilder AddParameters(IEnumerable<QueryParameter> parameters)
		{
			foreach (QueryParameter parameter in parameters)
			{
				AddParameter(parameter);
			}
			return this;
		}

		/// <summary>
		/// Gets the query string.
		/// </summary>
		public String GetQuery(String separator = "&")
		{
			return String.Join(separator, parameters.Select(p => p.Query));
		}
	}
}
