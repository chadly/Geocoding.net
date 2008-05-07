using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.XPath;

namespace GeoCoding.Services.Google
{
    public class GoogleGeoCoder : IGeoCoder
    {
        public const string ServiceUrl = "http://maps.google.com/maps/geo?output=xml&q={0}&key={1}&oe=utf8";

        private string _accessKey;
        private XmlNamespaceManager _namespaceManager;

        public string AccessKey
        {
            get { return _accessKey; }
        }

        public GoogleGeoCoder(string accessKey)
        {
            if (String.IsNullOrEmpty(accessKey)) throw new ArgumentNullException("accessKey");
            _accessKey = accessKey;
        }

        #region Xml Parsing

        private XmlNamespaceManager CreateXmlNamespaceManager(XPathNavigator nav)
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(nav.NameTable);
            nsManager.AddNamespace("kml", "http://earth.google.com/kml/2.0");
            nsManager.AddNamespace("adr", "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0");
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

        private XPathNavigator CreateSubNavigator(XPathNavigator nav)
        {
            using (StringReader reader = new StringReader(nav.OuterXml))
            {
                XPathDocument doc = new XPathDocument(reader);
                return doc.CreateNavigator();
            }
        }

        private string EvaluateXPath(string xpath, XPathNavigator nav)
        {
            XPathExpression exp = nav.Compile(xpath);
            exp.SetContext(_namespaceManager);
            return (string)nav.Evaluate(exp);
        }

        private Location FromCoordinates(string[] coordinates)
        {
            double longitude = double.Parse(coordinates[0]);
            double latitude = double.Parse(coordinates[1]);
            Location gpsCoordinates = new Location(latitude, longitude);
            return gpsCoordinates;
        }

        private Address RetrieveAddress(XPathNavigator nav)
        {
            //create a "sub-navigator" so that we can perform global xpath searches for nodes (e.g. //adr:PostalCodeNumber)
            //doing this because the xml schema changes depending upon the accuracy of the address returned
            //it is a pain in the ass to parse
            nav = CreateSubNavigator(nav);

            GoogleAddressAccuracy accuracy = (GoogleAddressAccuracy)int.Parse(EvaluateXPath("string(//adr:AddressDetails/@Accuracy)", nav));

            string formattedAddress = EvaluateXPath("string(//kml:address)", nav);
            string country = EvaluateXPath("string(//adr:CountryNameCode)", nav);
            string state = EvaluateXPath("string(//adr:AdministrativeAreaName)", nav);
            string county = EvaluateXPath("string(//adr:SubAdministrativeAreaName)", nav);
            string city = EvaluateXPath("string(//adr:LocalityName)", nav);
            string street = EvaluateXPath("string(//adr:ThoroughfareName)", nav);
            string zip = EvaluateXPath("string(//adr:PostalCodeNumber)", nav);
            string[] coordinates = EvaluateXPath("string(//kml:Point/kml:coordinates)", nav).Split(',');

            return new Address(street, city, state, zip, (Country)Enum.Parse(typeof(Country), country, true), FromCoordinates(coordinates), MapAccuracy(accuracy));
        }

        private Address[] ProcessWebResponse(WebResponse response)
        {
            XPathDocument xmlDoc = LoadXmlResponse(response);
            XPathNavigator nav = xmlDoc.CreateNavigator();
            _namespaceManager = CreateXmlNamespaceManager(nav);

            GoogleStatusCode status = (GoogleStatusCode)int.Parse(EvaluateXPath("string(kml:kml/kml:Response/kml:Status/kml:code)", nav));

            List<Address> addresses = new List<Address>();
            if (status == GoogleStatusCode.Success)
            {
                XPathExpression exp = nav.Compile("kml:kml/kml:Response/kml:Placemark");
                exp.SetContext(_namespaceManager);
                XPathNodeIterator nodes = nav.Select(exp);

                while (nodes.MoveNext())
                {
                    addresses.Add(RetrieveAddress(nodes.Current));
                }
            }

            return addresses.ToArray();
        }

        #endregion

        private AddressAccuracy MapAccuracy(GoogleAddressAccuracy accuracy)
        {
            switch (accuracy)
            {
                case GoogleAddressAccuracy.UnknownLocation: return AddressAccuracy.Unknown;
                case GoogleAddressAccuracy.CountryLevel: return AddressAccuracy.CountryLevel;
                case GoogleAddressAccuracy.RegionLevel: return AddressAccuracy.StateLevel;
                case GoogleAddressAccuracy.SubRegionLevel: return AddressAccuracy.StateLevel;
                case GoogleAddressAccuracy.TownLevel: return AddressAccuracy.CityLevel;
                case GoogleAddressAccuracy.ZipCodeLevel: return AddressAccuracy.PostalCodeLevel;
                case GoogleAddressAccuracy.StreetLevel: return AddressAccuracy.StreetLevel;
                case GoogleAddressAccuracy.IntersectionLevel: return AddressAccuracy.StreetLevel;
                case GoogleAddressAccuracy.AddressLevel: return AddressAccuracy.AddressLevel;
                default: return AddressAccuracy.Unknown;
            }
        }

        private HttpWebRequest BuildWebRequest(string address)
        {
            string url = String.Format(ServiceUrl, HttpUtility.UrlEncode(address), _accessKey);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            return req;
        }

        public Address[] GeoCode(string address)
        {
            if (String.IsNullOrEmpty(address)) throw new ArgumentNullException("address");

            HttpWebRequest request = BuildWebRequest(address);
            using (WebResponse response = request.GetResponse())
            {
                return ProcessWebResponse(response);
            }
        }

        public Address[] GeoCode(string street, string city, string state, string postalCode, Country country)
        {
            string address;
            if (country != Country.Unspecified)
                address = String.Format("{0} {1}, {2} {3}, {4}", street, city, state, postalCode, country);
            else
                address = String.Format("{0} {1}, {2} {3}", street, city, state, postalCode);

            return GeoCode(address);
        }

        public Address[] Validate(Address address)
        {
            return GeoCode(address.Street, address.City, address.State, address.PostalCode, address.Country);
        }

        public override string ToString()
        {
            return String.Format("Google GeoCoder: {0}", _accessKey);
        }
    }
}
