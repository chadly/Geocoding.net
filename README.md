# Generic C# Geocoding API [![Build Status](http://teamcity.chadly.net/app/rest/builds/buildType:(id:GeocodingNe_Build)/statusIcon)](http://teamcity.chadly.net/viewType.html?buildTypeId=GeocodingNe_Build&guest=1)

Includes a model and interface for communicating with five popular Geocoding providers.  Current implementations include:

  * [Google Maps](https://developers.google.com/maps/) - [docs](https://developers.google.com/maps/documentation/geocoding/)
  * [Yahoo! BOSS Geo Services](http://developer.yahoo.com/boss/geo/) - [docs](http://developer.yahoo.com/geo/placefinder/guide/index.html)
  * [Bing Maps (aka Virtual Earth)](http://www.microsoft.com/maps/) - [docs](http://msdn.microsoft.com/en-us/library/ff701715.aspx)
  * :warning: MapQuest [(Commercial API)](http://www.mapquestapi.com/) - [docs](http://www.mapquestapi.com/geocoding/)
  * :warning: MapQuest [(OpenStreetMap)](http://open.mapquestapi.com/) - [docs](http://open.mapquestapi.com/geocoding/)

The API returns latitude/longitude coordinates and normalized address information.  This can be used to perform address validation, real time mapping of user-entered addresses, distance calculations, and much more.

See latest [release notes](https://github.com/chadly/Geocoding.net/releases/latest).

:warning: There is a potential issue ([#29](https://github.com/chadly/Geocoding.net/issues/29)) regarding MapQuest that has a workaround. If you would like to help fix the issue, PRs are welcome.

## Installation

Install [via nuget](http://www.nuget.org/packages/Geocoding.net/):

```
Install-Package Geocoding.Core
```

and then choose which provider you want to install (or install all of them):

```
Install-Package Geocoding.Google
Install-Package Geocoding.MapQuest
Install-Package Geocoding.Microsoft
Install-Package Geocoding.Yahoo
```

## Example Usage

### Simple Example

```csharp
IGeocoder geocoder = new GoogleGeocoder() { ApiKey = "this-is-my-optional-google-api-key" };
IEnumerable<Address> addresses = await geocoder.GeocodeAsync("1600 pennsylvania ave washington dc");
Console.WriteLine("Formatted: " + addresses.First().FormattedAddress); //Formatted: 1600 Pennsylvania Ave SE, Washington, DC 20003, USA
Console.WriteLine("Coordinates: " + addresses.First().Coordinates.Latitude + ", " + addresses.First().Coordinates.Longitude); //Coordinates: 38.8791981, -76.9818437
```

It can also be used to return address information from latitude/longitude coordinates (aka reverse geocoding):

```csharp
IGeocoder geocoder = new YahooGeocoder("consumer-key", "consumer-secret");
IEnumerable<Address> addresses = await geocoder.ReverseGeocodeAsync(38.8976777, -77.036517);
```

### Using Provider-Specific Data

```csharp
GoogleGeocoder geocoder = new GoogleGeocoder();
IEnumerable<GoogleAddress> addresses = await geocoder.GeocodeAsync("1600 pennsylvania ave washington dc");

var country = addresses.Where(a => !a.IsPartialMatch).Select(a => a[GoogleAddressType.Country]).First();
Console.WriteLine("Country: " + country.LongName + ", " + country.ShortName); //Country: United States, US
```

The Microsoft and Yahoo implementations each provide their own address class as well, `BingAddress` and `YahooAddress`.

## API Keys

Google allows anonymous access to it's API, but if you start hitting rate limits, you must [sign up for a new Server API Key](https://developers.google.com/maps/documentation/javascript/tutorial#api_key).

Bing [requires an API key](http://msdn.microsoft.com/en-us/library/ff428642.aspx) to access its service.

You will need a [consumer secret and consumer key](http://developer.yahoo.com/boss/geo/BOSS_Signup.pdf) (PDF) for Yahoo.

MapQuest API requires a key. Sign up here: (http://developer.mapquest.com/web/products/open)

## How to Build from Source

```
dotnet restore
dotnet build
```

For a nice experience, use [Visual Studio Code](https://code.visualstudio.com/) to work with the project. The editor is cross platform and open source.

Alternatively, if you are on Windows, you can open the solution in [Visual Studio](https://www.visualstudio.com/) and build.

### Service Tests

You will need to generate API keys for each respective service to run the service tests. Make a `settings-override.json` as a copy of `settings.json` in the test project and put in your API keys. Then you should be able to run the tests.
