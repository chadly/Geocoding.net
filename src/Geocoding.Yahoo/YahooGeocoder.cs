using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Geocoding.Yahoo
{
	/// <remarks>
	/// http://developer.yahoo.com/geo/placefinder/
	/// </remarks>
	public class YahooGeocoder : IGeocoder
	{
		public const string ServiceUrl = "http://yboss.yahooapis.com/geo/placefinder?q={0}";
		public const string ServiceUrlNormal = "http://yboss.yahooapis.com/geo/placefinder?street={0}&city={1}&state={2}&postal={3}&country={4}";
		public const string ServiceUrlReverse = "http://yboss.yahooapis.com/geo/placefinder?q={0}&gflags=R";

		readonly string consumerKey, consumerSecret;

		public string ConsumerKey
		{
			get { return consumerKey; }
		}

		public string ConsumerSecret
		{
			get { return consumerSecret; }
		}

		public IWebProxy Proxy { get; set; }

		public YahooGeocoder(string consumerKey, string consumerSecret)
		{
			if (string.IsNullOrEmpty(consumerKey))
				throw new ArgumentNullException("consumerKey");

			if (string.IsNullOrEmpty(consumerSecret))
				throw new ArgumentNullException("consumerSecret");

			this.consumerKey = consumerKey;
			this.consumerSecret = consumerSecret;
		}

		public async Task<IEnumerable<YahooAddress>> GeocodeAsync(string address)
		{
			if (string.IsNullOrEmpty(address))
				throw new ArgumentNullException("address");

			string url = string.Format(ServiceUrl, WebUtility.UrlEncode(address));

			HttpWebRequest request = BuildWebRequest(url);
			return await ProcessRequest(request).ConfigureAwait(false);
		}

		public async Task<IEnumerable<YahooAddress>> GeocodeAsync(string street, string city, string state, string postalCode, string country)
		{
			string url = string.Format(ServiceUrlNormal, WebUtility.UrlEncode(street), WebUtility.UrlEncode(city), WebUtility.UrlEncode(state), WebUtility.UrlEncode(postalCode), WebUtility.UrlEncode(country));

			HttpWebRequest request = BuildWebRequest(url);
			return await ProcessRequest(request).ConfigureAwait(false);
		}

		public async Task<IEnumerable<YahooAddress>> ReverseGeocodeAsync(Location location)
		{
			if (location == null)
				throw new ArgumentNullException("location");

			return await ReverseGeocodeAsync(location.Latitude, location.Longitude).ConfigureAwait(false);
		}

		public async Task<IEnumerable<YahooAddress>> ReverseGeocodeAsync(double latitude, double longitude)
		{
			string url = string.Format(ServiceUrlReverse, string.Format(CultureInfo.InvariantCulture, "{0} {1}", latitude, longitude));

			HttpWebRequest request = BuildWebRequest(url);
			return await ProcessRequest(request).ConfigureAwait(false);
		}

		private async Task<IEnumerable<YahooAddress>> ProcessRequest(HttpWebRequest request)
		{
			try
			{
				using (WebResponse response = await request.GetResponseAsync().ConfigureAwait(false))
				{
					return ProcessWebResponse(response);
				}
			}
			catch (YahooGeocodingException)
			{
				//let these pass through
				throw;
			}
			catch (Exception ex)
			{
				//wrap in yahoo exception
				throw new YahooGeocodingException(ex);
			}
		}

		async Task<IEnumerable<Address>> IGeocoder.GeocodeAsync(string address)
		{
			return await GeocodeAsync(address).ConfigureAwait(false);
		}

		async Task<IEnumerable<Address>> IGeocoder.GeocodeAsync(string street, string city, string state, string postalCode, string country)
		{
			return await GeocodeAsync(street, city, state, postalCode, country).ConfigureAwait(false);
		}

		async Task<IEnumerable<Address>> IGeocoder.ReverseGeocodeAsync(Location location)
		{
			return await ReverseGeocodeAsync(location).ConfigureAwait(false);
		}

		async Task<IEnumerable<Address>> IGeocoder.ReverseGeocodeAsync(double latitude, double longitude)
		{
			return await ReverseGeocodeAsync(latitude, longitude).ConfigureAwait(false);
		}

		private HttpWebRequest BuildWebRequest(string url)
		{
			url = GenerateOAuthSignature(new Uri(url));
			var req = WebRequest.Create(url) as HttpWebRequest;
			req.Method = "GET";
			if (this.Proxy != null)
			{
				req.Proxy = this.Proxy;
			}
			return req;
		}

		string GenerateOAuthSignature(Uri uri)
		{
			string url, param;

			var oAuth = new OAuthBase();
			var nonce = oAuth.GenerateNonce();
			var timeStamp = oAuth.GenerateTimeStamp();

			var signature = oAuth.GenerateSignature(
				uri,
				consumerKey,
				consumerSecret,
				string.Empty,
				string.Empty,
				"GET",
				timeStamp,
				nonce,
				OAuthBase.SignatureTypes.HMACSHA1,
				out url,
				out param
			);

			return string.Format("{0}?{1}&oauth_signature={2}", url, param, signature);
		}

		private IEnumerable<YahooAddress> ProcessWebResponse(WebResponse response)
		{
			XPathDocument xmlDoc = LoadXmlResponse(response);
			XPathNavigator nav = xmlDoc.CreateNavigator();

			YahooError error = EvaluateError(Convert.ToInt32(nav.Evaluate("number(/ResultSet/Error)")));

			if (error != YahooError.NoError)
				throw new YahooGeocodingException(error);

			return ParseAddresses(nav.Select("/ResultSet/Result")).ToArray();
		}

		private XPathDocument LoadXmlResponse(WebResponse response)
		{
			using (Stream stream = response.GetResponseStream())
			{
				XPathDocument doc = new XPathDocument(stream);
				return doc;
			}
		}

		private IEnumerable<YahooAddress> ParseAddresses(XPathNodeIterator nodes)
		{
			while (nodes.MoveNext())
			{
				XPathNavigator nav = nodes.Current;

				int quality = Convert.ToInt32(nav.Evaluate("number(quality)"));
				string formattedAddress = ParseFormattedAddress(nav);

				double latitude = (double)nav.Evaluate("number(latitude)");
				double longitude = (double)nav.Evaluate("number(longitude)");
				Location coordinates = new Location(latitude, longitude);

				string name = (string)nav.Evaluate("string(name)");
				string house = (string)nav.Evaluate("string(house)");
				string street = (string)nav.Evaluate("string(street)");
				string unit = (string)nav.Evaluate("string(unit)");
				string unitType = (string)nav.Evaluate("string(unittype)");
				string neighborhood = (string)nav.Evaluate("string(neighborhood)");
				string city = (string)nav.Evaluate("string(city)");
				string county = (string)nav.Evaluate("string(county)");
				string countyCode = (string)nav.Evaluate("string(countycode)");
				string state = (string)nav.Evaluate("string(state)");
				string stateCode = (string)nav.Evaluate("string(statecode)");
				string postalCode = (string)nav.Evaluate("string(postal)");
				string country = (string)nav.Evaluate("string(country)");
				string countryCode = (string)nav.Evaluate("string(countrycode)");

				yield return new YahooAddress(
					formattedAddress,
					coordinates,
					name,
					house,
					street,
					unit,
					unitType,
					neighborhood,
					city,
					county,
					countyCode,
					state,
					stateCode,
					postalCode,
					country,
					countryCode,
					quality
				);
			}
		}

		private string ParseFormattedAddress(XPathNavigator nav)
		{
			string[] lines = new string[4];
			lines[0] = (string)nav.Evaluate("string(line1)");
			lines[1] = (string)nav.Evaluate("string(line2)");
			lines[2] = (string)nav.Evaluate("string(line3)");
			lines[3] = (string)nav.Evaluate("string(line4)");

			lines = lines.Select(s => (s ?? "").Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
			return string.Join(", ", lines);
		}

		private YahooError EvaluateError(int errorCode)
		{
			if (errorCode >= 1000)
				return YahooError.UnknownError;

			return (YahooError)errorCode;
		}

		public override string ToString()
		{
			return string.Format("Yahoo Geocoder: {0}, {1}", consumerKey, consumerSecret);
		}
	}
}