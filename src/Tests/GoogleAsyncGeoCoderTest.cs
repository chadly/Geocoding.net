using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoCoding.Google;
using Xunit.Extensions;
using Xunit;
using System.Net;

namespace GeoCoding.Tests
{
    public class GoogleAsyncGeoCoderTest : AsyncGeoCoderTest
    {
        GoogleGeoCoder geoCoder;

        protected override IAsyncGeoCoder CreateAsyncGeoCoder()
        {
            geoCoder = new GoogleGeoCoder();
            return geoCoder;
        }

        [Theory]
        [InlineData("United States", GoogleAddressType.Country)]
        [InlineData("Illinois, US", GoogleAddressType.AdministrativeAreaLevel1)]
        [InlineData("New York, New York", GoogleAddressType.Locality)]
        [InlineData("90210, US", GoogleAddressType.PostalCode)]
        [InlineData("1600 pennsylvania ave washington dc", GoogleAddressType.StreetAddress)]
        public void CanParseAddressTypes(string address, GoogleAddressType type)
        {
            geoCoder.GeoCodeAsync(address).ContinueWith(task =>
            {
                GoogleAddress[] addresses = task.Result.ToArray();
                Assert.Equal(type, addresses[0].Type);
            });
        }
    }
}
