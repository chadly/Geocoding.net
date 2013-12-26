#Generic C# Geocoding API

Includes a model and interface for communicating with three popular Geocoding providers.  Current implementations include:

  * [Google Maps](https://developers.google.com/maps/) - [docs](https://developers.google.com/maps/documentation/geocoding/)
  * [Yahoo! BOSS Geo Services](http://developer.yahoo.com/boss/geo/) - [docs](http://developer.yahoo.com/geo/placefinder/guide/index.html)
  * [Bing Maps (aka Virtual Earth)](http://www.microsoft.com/maps/) - [docs](http://msdn.microsoft.com/en-us/library/ff701715.aspx)

The API returns latitude/longitude coordinates and normalized address information.  This can be used to perform address validation, real time mapping of user-entered addresses, distance calculations, and much more.

See latest [release notes](https://github.com/chadly/Geocoding.net/wiki/Release-Notes).

##Installation

Install via nuget:

```
Install-Package Geocoding.net
```

Or download the [latest release](https://github.com/chadly/Geocoding.net/releases) and add a reference to `Geocoding.dll` in your project.

##Simple Example

```csharp
IGeocoder geocoder = new GoogleGeocoder() { ApiKey = "this-is-my-optional-google-api-key" };
Address[] addresses = geocoder.Geocode("123 Main St");
```

It can also be used to return address information from latitude/longitude coordinates (aka reverse geocoding):

```csharp
IGeocoder geocoder = new YahooGeocoder("consumer-key", "consumer-secret");
Address[] addresses = geocoder.ReverseGeocode(38.8976777, -77.036517);
```

###More Examples

A more in-depth runnable example of how this library can be integrated into an MVC4 application can be found in the [examples folder](https://github.com/chadly/Geocoding.net/tree/master/examples) of this repository. Download it and run locally.


##API Keys

Google allows anonymous access to it's API, but if you start hitting rate limits, you must [sign up for a new Server API Key](https://developers.google.com/maps/documentation/javascript/tutorial#api_key).

Bing [requires an API key](http://msdn.microsoft.com/en-us/library/ff428642.aspx) to access its service.

You will need a [consumer secret and consumer key](http://developer.yahoo.com/boss/geo/BOSS_Signup.pdf) (PDF) for Yahoo.


##How to Build from Source

In order to compile the solution in Visual Studio, you must first run build.bat. This will run a basic Debug build without running any tests. The build process generates some files that are needed to compile in Visual Studio.

### Service Tests
You will need to generate API keys for each respective service to run the service tests. Edit App.config in the Tests project and put in your API keys.

------------------------------------------

Help support development: `1K33yhGwx3zLopyJuAHWDn8XrMjM6Twwr8`

[![Bitdeli Badge](https://d2weczhvl823v0.cloudfront.net/chadly/geocoding.net/trend.png)](https://bitdeli.com/free "Bitdeli Badge")
