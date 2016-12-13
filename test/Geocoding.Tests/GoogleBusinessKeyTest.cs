using System;
using Geocoding.Google;
using Xunit;
using Xunit.Extensions;

namespace Geocoding.Tests
{
	public class GoogleBusinessKeyTest
	{
		[Fact]
		public void Should_throw_exception_on_null_client_id()
		{
			Assert.Throws<ArgumentNullException>(delegate
			{
				new BusinessKey(null, "signing-key");
			});
		}

		[Fact]
		public void Should_throw_exception_on_null_signing_key()
		{
			Assert.Throws<ArgumentNullException>(delegate
			{
				new BusinessKey("client-id", null);
			});
		}

		[Fact]
		public void Should_trim_client_id_and_signing_key()
		{
			var key = new BusinessKey("  client-id    ", " signing-key   ");

			Assert.Equal("client-id", key.ClientId);
			Assert.Equal("signing-key", key.SigningKey);
		}

		[Fact]
		public void Should_be_equal_by_value()
		{
			var key1 = new BusinessKey("client-id", "signing-key");
			var key2 = new BusinessKey("client-id", "signing-key");

			Assert.Equal(key1, key2);
			Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
		}

		[Fact]
		public void Should_not_be_equal_with_different_client_ids()
		{
			var key1 = new BusinessKey("client-id1", "signing-key");
			var key2 = new BusinessKey("client-id2", "signing-key");

			Assert.NotEqual(key1, key2);
			Assert.NotEqual(key1.GetHashCode(), key2.GetHashCode());
		}

		[Fact]
		public void Should_not_be_equal_with_different_signing_keys()
		{
			var key1 = new BusinessKey("client-id", "signing-key1");
			var key2 = new BusinessKey("client-id", "signing-key2");

			Assert.NotEqual(key1, key2);
			Assert.NotEqual(key1.GetHashCode(), key2.GetHashCode());
		}

		[Fact]
		public void Should_generate_signature_from_url()
		{
			var key = new BusinessKey("clientID", "vNIXE0xscrmjlyV-12Nj_BvUPaw=");

			string signedUrl = key.GenerateSignature("http://maps.googleapis.com/maps/api/geocode/json?address=New+York&sensor=false&client=clientID");

			Assert.NotNull(signedUrl);
			Assert.Equal("http://maps.googleapis.com/maps/api/geocode/json?address=New+York&sensor=false&client=clientID&signature=KrU1TzVQM7Ur0i8i7K3huiw3MsA=", signedUrl);
		}

		[Theory]
		[InlineData("   Channel_1   ")]
		[InlineData(" channel-1")]
		[InlineData("CUSTOMER ")]
		public void Should_trim_and_lower_channel_name(string channel)
		{
			var key = new BusinessKey("client-id", "signature", channel);
			Assert.Equal(channel.Trim().ToLower(), key.Channel);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("channel_1-2.")]
		public void Doesnt_throw_exception_on_alphanumeric_perioric_underscore_hyphen_character_in_channel(string channel)
		{
			new BusinessKey("client-id", "signature", channel);
		}

		[Theory]
		[InlineData("channel 1")]
		[InlineData("channel&1")]
		public void Should_throw_exception_on_special_characters_in_channel(string channel)
		{
			Assert.Throws<ArgumentException>(delegate
			{
				new BusinessKey("client-id", "signature", channel);
			});
		}

		[Fact]
		public void ServiceUrl_should_contains_channel_name()
		{
			var channel = "channel1";
			var key = new BusinessKey("client-id", "signature", channel);
			var geocoder = new GoogleGeocoder(key);

			Assert.Contains("channel="+channel, geocoder.ServiceUrl);
		}

		[Fact]
		public void ServiceUrl_doesnt_contains_channel_on_apikey()
		{
			var geocoder = new GoogleGeocoder("apikey");

			Assert.DoesNotContain("channel=", geocoder.ServiceUrl);
		}
	}
}