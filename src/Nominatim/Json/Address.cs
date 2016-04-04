using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace Json
{
    [DataContract]
    public class Address
    {
        [DataMember(Name = "house_number")]
        public string HouseNumber { get; set; }
        [DataMember(Name = "road")]
        public string Road { get; set; }
        [DataMember(Name = "suburb")]
        public string Suburb { get; set; }
        [DataMember(Name = "hamlet")]
        public string Hamlet { get; set; }
        [DataMember(Name = "city")]
        public string City { get; set; }
        [DataMember(Name = "state_district")]
        public string StateDistrict { get; set; }
        [DataMember(Name = "state")]
        public string State { get; set; }
        [DataMember(Name = "postcode")]
        public string Postcode { get; set; }
        [DataMember(Name = "country")]
        public string Country { get; set; }
        [DataMember(Name = "country_code")]
        public string CountryCode { get; set; }
    }
}