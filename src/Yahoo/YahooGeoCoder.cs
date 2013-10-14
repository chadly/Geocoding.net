using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.XPath;

namespace GeoCoding.Yahoo
{
	/// <remarks>
	/// http://developer.yahoo.com/geo/placefinder/
	/// </remarks>
	public class YahooGeoCoder : IGeoCoder
	{
		public const string ServiceUrl = "http://where.yahooapis.com/geocode?q={0}&appid={1}";		
		public const string ServiceUrlNormal = "http://where.yahooapis.com/geocode?street={0}&city={1}&state={2}&postal={3}&country={4}&appid={5}";
		public const string ServiceUrlReverse = "http://where.yahooapis.com/geocode?q={0}&appid={1}&gflags=R";

		readonly string appId;

		public string AppId
		{
			get { return appId; }
		}

		public YahooGeoCoder(string appId)
		{
			if (String.IsNullOrEmpty(appId))
				throw new ArgumentNullException("appId");

			this.appId = appId;
		}

		public IEnumerable<YahooAddress> GeoCode(string address)
		{
			if (String.IsNullOrEmpty(address))
				throw new ArgumentNullException("address");

			string url = String.Format(ServiceUrl, HttpUtility.UrlEncode(address), appId);

			HttpWebRequest request = BuildWebRequest(url);
			return ProcessRequest(request);
		}

		public IEnumerable<YahooAddress> GeoCode(string street, string city, string state, string postalCode, string country)
		{
			string url = String.Format(ServiceUrlNormal, HttpUtility.UrlEncode(street), HttpUtility.UrlEncode(city), HttpUtility.UrlEncode(state), HttpUtility.UrlEncode(postalCode), HttpUtility.UrlEncode(country), appId);

			HttpWebRequest request = BuildWebRequest(url);
			return ProcessRequest(request);
		}

		public IEnumerable<YahooAddress> ReverseGeoCode(Location location)
		{
			if (location == null)
				throw new ArgumentNullException("location");

			return ReverseGeoCode(location.Latitude, location.Longitude);
		}

		public IEnumerable<YahooAddress> ReverseGeoCode(double latitude, double longitude)
		{
			string url = String.Format(ServiceUrlReverse, String.Format(CultureInfo.InvariantCulture, "{0} {1}", latitude, longitude), appId);

			HttpWebRequest request = BuildWebRequest(url);
			return ProcessRequest(request);
		}

		private IEnumerable<YahooAddress> ProcessRequest(HttpWebRequest request)
		{
			try
			{
				using (WebResponse response = request.GetResponse())
				{
					return ProcessWebResponse(response);
				}
			}
			catch (YahooGeoCodingException)
			{
				//let these pass through
				throw;
			}
			catch (Exception ex)
			{
				//wrap in yahoo exception
				throw new YahooGeoCodingException(ex);
			}
		}

		IEnumerable<Address> IGeoCoder.GeoCode(string address)
		{
			return GeoCode(address).Cast<Address>();
		}

		IEnumerable<Address> IGeoCoder.GeoCode(string street, string city, string state, string postalCode, string country)
		{
			return GeoCode(street, city, state, postalCode, country).Cast<Address>();
		}

		IEnumerable<Address> IGeoCoder.ReverseGeocode(Location location)
		{
			return ReverseGeoCode(location).Cast<Address>();
		}

		IEnumerable<Address> IGeoCoder.ReverseGeocode(double latitude, double longitude)
		{
			return ReverseGeoCode(latitude, longitude).Cast<Address>();
		}

		private HttpWebRequest BuildWebRequest(string url)
		{
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.Method = "GET";
			return req;
		}

		private IEnumerable<YahooAddress> ProcessWebResponse(WebResponse response)
		{
			XPathDocument xmlDoc = LoadXmlResponse(response);
			XPathNavigator nav = xmlDoc.CreateNavigator();

			YahooError error = EvaluateError(Convert.ToInt32(nav.Evaluate("number(/ResultSet/Error)")));

			if (error != YahooError.NoError)
				throw new YahooGeoCodingException(error);

			return ParseAddresses(nav.Select("/ResultSet/Result"));
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

			lines = lines.Select(s => (s ?? "").Trim()).Where(s => !String.IsNullOrEmpty(s)).ToArray();
			return String.Join(", ", lines);
		}

		private YahooError EvaluateError(int errorCode)
		{
			if (errorCode >= 1000)
				return YahooError.UnknownError;

			return (YahooError)errorCode;
		}

		public override string ToString()
		{
			return String.Format("Yahoo GeoCoder: {0}", appId);
		}
	}
}