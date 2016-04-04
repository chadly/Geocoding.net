using Geocoding.GeoNames.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Geocoding.GeoNames
{
    public class GeoNamesGeocoder : IGeocoder
    {
        #region Private Fields

        private const string REVERSE = "findNearby?type=json";
        private const string SEARCH = "search?type=json";
        private const string URL = "http://api.geonames.org/";

        #endregion Private Fields

        #region Public Properties

        public string Language { get; set; }

        public IWebProxy Proxy { get; set; }

        public string UserName { get; set; }

        #endregion Public Properties

        #region Public Methods

        IEnumerable<Address> IGeocoder.Geocode(string address)
        {
            return Geocode(address).Cast<Address>();
        }

        IEnumerable<Address> IGeocoder.Geocode(string street, string city, string state, string postalCode, string country)
        {
            throw new NotSupportedException("Search by street, city, state, postal code and country is not supported.");
        }

        IEnumerable<Address> IGeocoder.ReverseGeocode(Location location)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            return ReverseGeocode(location.Latitude, location.Longitude);
        }

        IEnumerable<Address> IGeocoder.ReverseGeocode(double latitude, double longitude)
        {
            return ReverseGeocode(latitude, longitude);
        }

        #endregion Public Methods

        #region Private Methods

        public static string StreamToString(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private HttpWebRequest BuildWebRequest(string url)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            if (this.Proxy != null)
            {
                request.Proxy = Proxy;
            }
            request.Method = "GET";

            return request;
        }

        private IEnumerable<GeoNamesAddress> Geocode(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            var url = GetQueryUrl(address);
            HttpWebRequest request = BuildWebRequest(url);
            var roots = ProcessRequest<RootObject>(request);
            return roots.Geonames.Select(ParseResonse);
        }

        private string GetQueryUrl(string address)
        {
            var url = GetSearchUrl();

            url.AddParameterIfNotNullOrEmpty("q", address);

            return url.ToString();
        }

        private string GetQueryUrl(double latitude, double longitude)
        {
            var url = GetReverseUrl();

            url.AddParameterIfNotNullOrEmpty("lat", latitude.ToString(CultureInfo.InvariantCulture));
            url.AddParameterIfNotNullOrEmpty("lng", longitude.ToString(CultureInfo.InvariantCulture));

            return url.ToString();
        }

        private StringBuilder GetReverseUrl()
        {
            var builder = new StringBuilder(URL);

            builder.Append(REVERSE);
            builder.AddParameterIfNotNullOrEmpty("lang", Language);
            builder.AddParameterIfNotNullOrEmpty("username", UserName);

            return builder;
        }

        private StringBuilder GetSearchUrl()
        {
            var builder = new StringBuilder(URL);

            builder.Append(SEARCH);
            builder.AddParameterIfNotNullOrEmpty("searchlang", Language);
            builder.AddParameterIfNotNullOrEmpty("username", UserName);
            //builder.AddParameterIfNotNullOrEmpty("countrycodes", CountryCodes.StringJoin(","));

            //if (BoundsBias != null)
            //{
            //    //viewbox=<left>,<top>,<right>,<bottom>
            //    builder.AddParameterIfNotNullOrEmpty("viewbox", new[]
            //    {
            //        BoundsBias.SouthWest.Longitude.ToString(CultureInfo.InvariantCulture),//Left
            //        BoundsBias.NorthEast.Latitude.ToString(CultureInfo.InvariantCulture),//Top
            //        BoundsBias.NorthEast.Longitude.ToString(CultureInfo.InvariantCulture),//Right
            //        BoundsBias.SouthWest.Latitude.ToString(CultureInfo.InvariantCulture)//Bottom
            //    }.StringJoin(","));
            //}

            return builder;
        }

        private GeoNamesAddress ParseResonse(Geoname geoName)
        {
            var location = new Location(geoName.Lat, geoName.Lng);
            return new GeoNamesAddress(geoName.ToString(), location)
            {
                GeonameId = geoName.GeonameId,

                ASCII = geoName.ASCIIName,

                AdminCode1 = geoName.AdminCode1,
                AdminCode2 = geoName.AdminCode2,
                AdminCode3 = geoName.AdminCode3,
                AdminId1 = geoName.AdminId1,
                AdminId2 = geoName.AdminId2,
                AdminId3 = geoName.AdminId3,
                AdminName1 = geoName.AdminName1,
                AdminName2 = geoName.AdminName2,
                AdminName3 = geoName.AdminName3,
                AdminName4 = geoName.AdminName4,
                AdminName5 = geoName.AdminName5,
                AlternateNames = geoName.AlternateNames,

                BoundingBox = geoName.BoundingBox,
                ContinentCode = geoName.ContinentCode,

                CountryId = geoName.CountryId,
                CountryName = geoName.CountryName,
                CountryCode = geoName.CountryCode,

                Fcl = geoName.Fcl,
                FclName = geoName.FclName,
                Fcode = geoName.Fcode,
                FcodeName = geoName.FcodeName,

                Population = geoName.Population,
                Score = geoName.Score,
                Timezone = geoName.Timezone,
                ToponymName = geoName.ToponymName,
            };
        }

        private T ProcessRequest<T>(HttpWebRequest request)
        {
            try
            {
                using (WebResponse response = request.GetResponseAsync().Result)
                using (var stream = response.GetResponseStream())
                {
                    var streamResult = StreamToString(stream);
                    var root = JsonConvert.DeserializeObject<T>(streamResult);
                    
                    return root;
                }
            }
            catch (GeoNamesGeocodingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new GeoNamesGeocodingException("Can't process the request, see inner exception!", ex);
            }
        }

        private IEnumerable<Address> ReverseGeocode(double latitude, double longitude)
        {
            var url = GetQueryUrl(latitude, longitude);
            HttpWebRequest request = BuildWebRequest(url);

            var root = ProcessRequest<RootObject>(request);

            return root.Geonames.Select(ParseResonse);
        }

        #endregion Private Methods
    }
}