using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace Geocoding.Tests
{
	public abstract class BatchGeocoderTest
	{
		readonly IBatchGeocoder batchGeocoder;

		public BatchGeocoderTest() 
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");

			batchGeocoder = CreateBatchGeocoder();
		}

		protected abstract IBatchGeocoder CreateBatchGeocoder();

		[Theory]
		[PropertyData("BatchGeoCodeData")]
		public virtual void CanGeoCodeAddress(string[] addresses)
		{
			Assert.NotEmpty(addresses);

			ICollection<ResultItem> results = batchGeocoder.Geocode(addresses);
			Assert.NotEmpty(results);
			Assert.Equal(addresses.Length, results.Count);

			var ahash = new HashSet<string>(addresses);
			Assert.Equal(ahash.Count, results.Count);

			foreach (ResultItem r in results)
			{
				Assert.NotNull(r);
				Assert.NotNull(r.Request);
				Assert.NotNull(r.Response);

				Assert.Contains(r.Request.FormattedAddress, ahash);

				Address[] respa = r.Response.ToArray();
				Assert.NotEmpty(respa);

				ahash.Remove(r.Request.FormattedAddress);
			}
			Assert.Empty(ahash);
		}

		public static IEnumerable<object[]> BatchGeoCodeData
		{
			get
			{
				yield return new object[] 
				{
					new string[] 
					{
						"1600 pennsylvania ave nw, washington dc",
						"1460 4th Street Ste 304, Santa Monica CA 90401",
					},
				};
			}
		}

	}
}
