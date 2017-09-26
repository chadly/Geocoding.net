using Microsoft.Extensions.Configuration;
using Xunit;

namespace Geocoding.Tests
{
	public class SettingsFixture
	{
		readonly IConfigurationRoot config;

		public SettingsFixture()
		{
			config = new ConfigurationBuilder()
				.AddJsonFile("settings.json")
				.AddJsonFile("settings-override.json", optional: true)
				.Build();
		}

		public string YahooConsumerKey
		{
			get { return config.GetValue<string>("yahooConsumerKey"); }
		}

		public string YahooConsumerSecret
		{
			get { return config.GetValue<string>("yahooConsumerSecret"); }
		}

		public string BingMapsKey
		{
			get { return config.GetValue<string>("bingMapsKey"); }
		}

		public string GoogleApiKey
		{
			get { return config.GetValue<string>("googleApiKey"); }
		}

		public string MapQuestKey
		{
			get { return config.GetValue<string>("mapQuestKey"); }
		}
	}

	[CollectionDefinition("Settings")]
	public class SettingsCollection : ICollectionFixture<SettingsFixture>
	{
		// https://xunit.github.io/docs/shared-context.html
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}
}
