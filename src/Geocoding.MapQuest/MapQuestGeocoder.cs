using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Geocoding.MapQuest
{
	/// <remarks>
	/// <see cref="http://open.mapquestapi.com/geocoding/"/>
	/// <seealso cref="http://developer.mapquest.com/"/>
	/// </remarks>
	public class MapQuestGeocoder : IGeocoder, IBatchGeocoder
	{
		readonly string key;

		volatile bool useOSM;
		/// <summary>
		/// When true, will use the Open Street Map API
		/// </summary>
		public virtual bool UseOSM
		{
			get { return useOSM; }
			set { useOSM = value; }
		}

		public IWebProxy Proxy { get; set; }

		public MapQuestGeocoder(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentException("key can not be null or blank");

			this.key = key;
		}

		IEnumerable<Address> HandleSingleResponse(MapQuestResponse res)
		{
			if (res != null && !res.Results.IsNullOrEmpty())
			{
				return HandleSingleResponse(from r in res.Results
											where r != null && !r.Locations.IsNullOrEmpty()
											from l in r.Locations
											select l);
			}
			else
				return new Address[0];
		}

		IEnumerable<Address> HandleSingleResponse(IEnumerable<MapQuestLocation> locs)
		{
			if (locs == null)
				return new Address[0];
			else
			{
				return from l in locs
					   where l != null && l.Quality < Quality.COUNTRY
					   let q = (int)l.Quality
					   let c = string.IsNullOrWhiteSpace(l.Confidence) ? "ZZZZZZ" : l.Confidence
					   orderby q ascending, c ascending
					   select l;
			}
		}

		public async Task<IEnumerable<Address>> GeocodeAsync(string address)
		{
			if (string.IsNullOrWhiteSpace(address))
				throw new ArgumentException("address can not be null or empty!");

			var f = new GeocodeRequest(key, address) { UseOSM = this.UseOSM };
			MapQuestResponse res = await Execute(f).ConfigureAwait(false);
			return HandleSingleResponse(res);
		}

		public async Task<IEnumerable<Address>> GeocodeAsync(string street, string city, string state, string postalCode, string country)
		{
			var sb = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(street))
				sb.AppendFormat("{0}, ", street);
			if (!string.IsNullOrWhiteSpace(city))
				sb.AppendFormat("{0}, ", city);
			if (!string.IsNullOrWhiteSpace(state))
				sb.AppendFormat("{0} ", state);
			if (!string.IsNullOrWhiteSpace(postalCode))
				sb.AppendFormat("{0} ", postalCode);
			if (!string.IsNullOrWhiteSpace(country))
				sb.AppendFormat("{0} ", country);

			if (sb.Length > 1)
				sb.Length--;

			string s = sb.ToString().Trim();
			if (string.IsNullOrWhiteSpace(s))
				throw new ArgumentException("Concatenated input values can not be null or blank");

			if (s.Last() == ',')
				s = s.Remove(s.Length - 1);

			return await GeocodeAsync(s).ConfigureAwait(false);
		}

		public async Task<IEnumerable<Address>> ReverseGeocodeAsync(Location location)
		{
			if (location == null)
				throw new ArgumentNullException("location");

			var f = new ReverseGeocodeRequest(key, location) { UseOSM = this.UseOSM };
			MapQuestResponse res = await Execute(f).ConfigureAwait(false);
			return HandleSingleResponse(res);
		}

		public async Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude)
		{
			return await ReverseGeocodeAsync(new Location(latitude, longitude)).ConfigureAwait(false);
		}

		public async Task<MapQuestResponse> Execute(BaseRequest f)
		{
			HttpWebRequest request = await Send(f).ConfigureAwait(false);
			MapQuestResponse r = await Parse(request).ConfigureAwait(false);
			if (r != null && !r.Results.IsNullOrEmpty())
			{
				foreach (MapQuestResult o in r.Results)
				{
					if (o == null)
						continue;

					foreach (MapQuestLocation l in o.Locations)
					{
						if (!string.IsNullOrWhiteSpace(l.FormattedAddress) || o.ProvidedLocation == null)
							continue;

						if (string.Compare(o.ProvidedLocation.FormattedAddress, "unknown", true) != 0)
							l.FormattedAddress = o.ProvidedLocation.FormattedAddress;
						else
							l.FormattedAddress = o.ProvidedLocation.ToString();
					}
				}
			}
			return r;
		}

		private async Task<HttpWebRequest> Send(BaseRequest f)
		{
			if (f == null)
				throw new ArgumentNullException("f");

			HttpWebRequest request;
			bool hasBody = false;
			switch (f.RequestVerb)
			{
				case "GET":
				case "DELETE":
				case "HEAD":
					{
						var u = string.Format("{0}json={1}&", f.RequestUri, WebUtility.UrlEncode(f.RequestBody));
						request = WebRequest.Create(u) as HttpWebRequest;
					}
					break;
				case "POST":
				case "PUT":
				default:
					{
						request = WebRequest.Create(f.RequestUri) as HttpWebRequest;
						hasBody = !string.IsNullOrWhiteSpace(f.RequestBody);
					}
					break;
			}
			request.Method = f.RequestVerb;
			request.ContentType = "application/" + f.InputFormat;

			if (Proxy != null)
				request.Proxy = Proxy;

			if (hasBody)
			{
				byte[] buffer = Encoding.UTF8.GetBytes(f.RequestBody);
				//request.Headers.ContentLength = buffer.Length;
				using (Stream rs = await request.GetRequestStreamAsync().ConfigureAwait(false))
				{
					rs.Write(buffer, 0, buffer.Length);
					rs.Flush();
				}
			}
			return request;
		}

		private async Task<MapQuestResponse> Parse(HttpWebRequest request)
		{
			if (request == null)
				throw new ArgumentNullException("request");

			string requestInfo = string.Format("[{0}] {1}", request.Method, request.RequestUri);
			try
			{
				string json;
				using (HttpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse)
				{
					if ((int)response.StatusCode >= 300) //error
						throw new Exception((int)response.StatusCode + " " + response.StatusDescription);

					using (var sr = new StreamReader(response.GetResponseStream()))
						json = sr.ReadToEnd();
				}
				if (string.IsNullOrWhiteSpace(json))
					throw new Exception("Remote system response with blank: " + requestInfo);

				MapQuestResponse o = json.FromJSON<MapQuestResponse>();
				if (o == null)
					throw new Exception("Unable to deserialize remote response: " + requestInfo + " => " + json);

				return o;
			}
			catch (WebException wex) //convert to simple exception & close the response stream
			{
				using (HttpWebResponse response = wex.Response as HttpWebResponse)
				{
					var sb = new StringBuilder(requestInfo);
					sb.Append(" | ");
					sb.Append(response.StatusDescription);
					sb.Append(" | ");
					using (var sr = new StreamReader(response.GetResponseStream()))
					{
						sb.Append(sr.ReadToEnd());
					}
					throw new Exception((int)response.StatusCode + " " + sb.ToString());
				}
			}
		}

		public async Task<IEnumerable<ResultItem>> GeocodeAsync(IEnumerable<string> addresses)
		{
			if (addresses == null)
				throw new ArgumentNullException("addresses");

			string[] adr = (from a in addresses
							where !string.IsNullOrWhiteSpace(a)
							group a by a into ag
							select ag.Key).ToArray();
			if (adr.IsNullOrEmpty())
				throw new ArgumentException("Atleast one none blank item is required in addresses");

			var f = new BatchGeocodeRequest(key, adr) { UseOSM = this.UseOSM };
			MapQuestResponse res = await Execute(f).ConfigureAwait(false);
			return HandleBatchResponse(res);
		}

		ICollection<ResultItem> HandleBatchResponse(MapQuestResponse res)
		{
			if (res != null && !res.Results.IsNullOrEmpty())
			{
				return (from r in res.Results
						where r != null && !r.Locations.IsNullOrEmpty()
						let resp = HandleSingleResponse(r.Locations)
						where resp != null
						select new ResultItem(r.ProvidedLocation, resp)).ToArray();
			}
			else
				return new ResultItem[0];
		}

		public Task<IEnumerable<ResultItem>> ReverseGeocodeAsync(IEnumerable<Location> locations)
		{
			throw new NotSupportedException("ReverseGeocodeAsync(...) is not available for MapQuestGeocoder.");
		}
	}
}