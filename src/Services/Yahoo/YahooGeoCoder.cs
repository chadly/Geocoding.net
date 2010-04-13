using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.XPath;

namespace GeoCoding.Services.Yahoo
{
    public class YahooGeoCoder : IGeoCoder
    {
        public const string ServiceUrl = "http://local.yahooapis.com/MapsService/V1/geocode?location={0}&appid={1}";
        public const string ServiceUrlNormal = "http://local.yahooapis.com/MapsService/V1/geocode?street={0}&city={1}&state={2}&zip={3}&appid={4}";

        private readonly string appId;
        private XmlNamespaceManager namespaceManager;

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

		private Address[] GeoCode(HttpWebRequest request)
		{
			try
			{
				using (WebResponse response = request.GetResponse())
				{
					return ProcessWebResponse(response);
				}
			}
			catch (WebException ex)
			{
				if (!HandleWebException(ex))
					throw;
				return new Address[0];
			}
		}

		public Address[] GeoCode(string address)
		{
			if (String.IsNullOrEmpty(address)) throw new ArgumentNullException("address");

			HttpWebRequest request = BuildWebRequest(address);
			return GeoCode(request);
		}

		public Address[] GeoCode(string street, string city, string state, string postalCode, string country)
		{
			//ignoring the country parameter since yahoo doesn't accept it
			HttpWebRequest request = BuildWebRequest(street, city, state, postalCode);
			return GeoCode(request);
		}

        private AddressAccuracy MapAccuracy(string precision)
        {
            switch (precision)
            {
                case "address": return AddressAccuracy.AddressLevel;
                case "street": return AddressAccuracy.StreetLevel;
                case "zip+4":
                case "zip+2":
                case "zip": return AddressAccuracy.PostalCodeLevel;
                case "city": return AddressAccuracy.CityLevel;
                case "state": return AddressAccuracy.StateLevel;
                case "country": return AddressAccuracy.CountryLevel;
                default: return AddressAccuracy.Unknown;
            }
        }

        private HttpWebRequest BuildWebRequest(string address)
        {
            string url = String.Format(ServiceUrl, HttpUtility.UrlEncode(address), appId);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            return req;
        }

        private HttpWebRequest BuildWebRequest(string street, string city, string state, string postalCode)
        {
            string url = String.Format(ServiceUrlNormal, HttpUtility.UrlEncode(street), HttpUtility.UrlEncode(city), HttpUtility.UrlEncode(state), HttpUtility.UrlEncode(postalCode), appId);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            return req;
        }

        private bool HandleWebException(WebException ex)
        {
            //yahoo returns a HTTP 400 Bad Request response when it gets an address it can't find
            if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadRequest)
                return true;
            return false;
        }

		#region XML Parsing

		private XmlNamespaceManager CreateXmlNamespaceManager(XPathNavigator nav)
		{
			XmlNamespaceManager nsManager = new XmlNamespaceManager(nav.NameTable);
			nsManager.AddNamespace("y", "urn:yahoo:maps");
			return nsManager;
		}

		private XPathDocument LoadXmlResponse(WebResponse response)
		{
			using (Stream stream = response.GetResponseStream())
			{
				XPathDocument doc = new XPathDocument(stream);
				return doc;
			}
		}

		private string EvaluateXPath(string xpath, XPathNavigator nav)
		{
			XPathExpression exp = nav.Compile(xpath);
			exp.SetContext(namespaceManager);
			return (string)nav.Evaluate(exp);
		}

		private Address RetrieveAddress(XPathNavigator nav)
		{
			AddressAccuracy accuracy = MapAccuracy(EvaluateXPath("string(@precision)", nav));

			double latitude = double.Parse(EvaluateXPath("string(y:Latitude)", nav), CultureInfo.InvariantCulture);
			double longitude = double.Parse(EvaluateXPath("string(y:Longitude)", nav), CultureInfo.InvariantCulture);

			string street = EvaluateXPath("string(y:Address)", nav);
			string city = EvaluateXPath("string(y:City)", nav);
			string state = EvaluateXPath("string(y:State)", nav);
			string postalCode = EvaluateXPath("string(y:Zip)", nav);
			string country = EvaluateXPath("string(y:Country)", nav);

			return new Address(
				street,
				city,
				state,
				postalCode,
				country,
				new Location(latitude, longitude),
				accuracy
			);
		}

		private Address[] ProcessWebResponse(WebResponse response)
		{
			XPathDocument xmlDoc = LoadXmlResponse(response);
			XPathNavigator nav = xmlDoc.CreateNavigator();
			namespaceManager = CreateXmlNamespaceManager(nav);

			XPathExpression exp = nav.Compile("y:ResultSet/y:Result");
			exp.SetContext(namespaceManager);
			XPathNodeIterator nodes = nav.Select(exp);

			List<Address> addresses = new List<Address>();
			while (nodes.MoveNext())
			{
				addresses.Add(RetrieveAddress(nodes.Current));
			}

			return addresses.ToArray();
		}

		#endregion

        public override string ToString()
        {
            return String.Format("Yahoo GeoCoder: {0}", appId);
        }
    }
}
