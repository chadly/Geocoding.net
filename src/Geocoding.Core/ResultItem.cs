using System;
using System.Collections.Generic;

namespace Geocoding
{
	public class ResultItem
	{
		Address input;
		/// <summary>
		/// Original input for this response
		/// </summary>
		public Address Request
		{
			get { return input; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("Input");

				input = value;
			}
		}

		IEnumerable<Address> output;
		/// <summary>
		/// Output for the given input
		/// </summary>
		public IEnumerable<Address> Response
		{
			get { return output; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("Response");

				output = value;
			}
		}

		public ResultItem(Address request, IEnumerable<Address> response)
		{
			Request = request;
			Response = response;
		}
	}
}
