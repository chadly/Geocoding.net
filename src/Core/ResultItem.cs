using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geocoding
{
	public class ResultItem
	{
		public ResultItem(Address request, IEnumerable<Address> response)
		{
			Request = request;
			Response = response;
		}

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
	}
}
