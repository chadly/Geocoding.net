using Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Geocoding.Nominatim
{
    public class NominatimGeocoder : IGeocoder
    {
        #region Private Fields

        private const string URL = "https://nominatim.openstreetmap.org/";
        private const string SEARCH = "search?format=json&addressdetails=1";
        private const string REVERSE = "reverse?format=json&addressdetails=1";

        #endregion Private Fields

        #region Public Constructors

        public NominatimGeocoder()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public Bounds BoundsBias { get; set; }

        /// <summary>
        /// Gets or sets the country codes by ISO 3166-1 alpha2.
        /// </summary>
        /// <see cref="http://de.wikipedia.org/wiki/ISO-3166-1-Kodierliste"/>
        /// <value>
        /// The country codes.
        /// </value>
        public IEnumerable<string> CountryCodes { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <see cref="https://tools.ietf.org/html/rfc2616"/>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }

        public IWebProxy Proxy { get; set; }

        #endregion Public Properties

        #region Public Methods

        IEnumerable<Address> IGeocoder.Geocode(string address)
        {
            return Geocode(address).Cast<Address>();
        }

        IEnumerable<Address> IGeocoder.Geocode(string street, string city, string state, string postalCode, string country)
        {
            return Geocode(street, city, state, postalCode, country).Cast<Address>();
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

        private IEnumerable<NominatimAddress> Geocode(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            var url = GetQueryUrl(address);
            HttpWebRequest request = BuildWebRequest(url);
            var roots = ProcessRequest<IEnumerable<RootObject>>(request);
            return roots.Select(r => ParseResonse(r));
        }

        private IEnumerable<NominatimAddress> Geocode(string street, string city, string state, string postalCode, string country)
        {
            var url = GetQueryUrl(street, city, state, postalCode, country);
            HttpWebRequest request = BuildWebRequest(url);
            var roots = ProcessRequest<IEnumerable<RootObject>>(request);
            return roots.Select(r => ParseResonse(r));
        }

        private StringBuilder GetSearchUrl()
        {
            var builder = new StringBuilder(URL);

            builder.Append(SEARCH);
            builder.AddParameterIfNotNullOrEmpty("accept-language", Language);
            builder.AddParameterIfNotNullOrEmpty("email", Email);
            builder.AddParameterIfNotNullOrEmpty("countrycodes", CountryCodes.StringJoin(","));

            if (BoundsBias != null)
            {
                //viewbox=<left>,<top>,<right>,<bottom>
                builder.AddParameterIfNotNullOrEmpty("viewbox", new[]
                {
                    BoundsBias.SouthWest.Longitude.ToString(CultureInfo.InvariantCulture),//Left
                    BoundsBias.NorthEast.Latitude.ToString(CultureInfo.InvariantCulture),//Top
                    BoundsBias.NorthEast.Longitude.ToString(CultureInfo.InvariantCulture),//Right
                    BoundsBias.SouthWest.Latitude.ToString(CultureInfo.InvariantCulture)//Bottom
                }.StringJoin(","));
            }

            return builder;
        }

        private StringBuilder GetReverseUrl()
        {
            var builder = new StringBuilder(URL);

            builder.Append(REVERSE);
            builder.AddParameterIfNotNullOrEmpty("accept-language", Language);
            builder.AddParameterIfNotNullOrEmpty("email", Email);

            return builder;
        }

        private string GetQueryUrl(string address)
        {
            var url = GetSearchUrl();

            url.AddParameterIfNotNullOrEmpty("q", address);

            return url.ToString();
        }

        private string GetQueryUrl(string street, string city, string state, string postalCode, string country)
        {
            var url = GetSearchUrl();

            url.AddParameterIfNotNullOrEmpty("street", street);
            url.AddParameterIfNotNullOrEmpty("city", city);
            url.AddParameterIfNotNullOrEmpty("state", state);
            url.AddParameterIfNotNullOrEmpty("postalcode", postalCode);
            url.AddParameterIfNotNullOrEmpty("country", country);

            return url.ToString();
        }

        private string GetQueryUrl(double latitude, double longitude)
        {
            var url = GetReverseUrl();

            url.AddParameterIfNotNullOrEmpty("lat", latitude.ToString(CultureInfo.InvariantCulture));
            url.AddParameterIfNotNullOrEmpty("lon", longitude.ToString(CultureInfo.InvariantCulture));
            url.AddParameterIfNotNullOrEmpty("zoom", 18.ToString());

            return url.ToString();
        }

        private NominatimAddress ParseResonse(RootObject o)
        {
            return new NominatimAddress(o.DisplayName, new Location(o.Latitude, o.Longitude))
            {
                City = o.Address.City,
                Country = o.Address.Country,
                CountryCode = o.Address.CountryCode,
                Hamlet = o.Address.Hamlet,
                HouseNumber = o.Address.HouseNumber,
                PostalCode = o.Address.Postcode,
                Road = o.Address.Road,
                State = o.Address.State,
                StateDistrict = o.Address.StateDistrict,
                Suburb = o.Address.Suburb,
            };
        }

        private IEnumerable<NominatimAddress> ReverseGeocode(double latitude, double longitude)
        {
            var url = GetQueryUrl(latitude, longitude);
            HttpWebRequest request = BuildWebRequest(url);

            var root = ProcessRequest<RootObject>(request);
            return new[] { ParseResonse(root) };
        }

        private T ProcessRequest<T>(HttpWebRequest request)
        {
            try
            {
                using (WebResponse response = request.GetResponseAsync().Result)
                {
                    var jsonSerializer = new DataContractJsonSerializer(typeof(T));
                    var stream = response.GetResponseStream();
                    var serialized = jsonSerializer.ReadObject(stream);
                    var root = (T)serialized;

                    return root;
                }
            }
            catch (NominatimGeocodingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NominatimGeocodingException("Can't process the request, see inner exception!",ex);
            }
        }

        #endregion Private Methods
    }
}