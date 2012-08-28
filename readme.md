Generic C# GeoCoding API
========================

Includes a portable GeoCoding domain model along with a generic IGeoCoder service interface.  Current implementations include:

  * [Google Maps](https://developers.google.com/maps/) - [docs](https://developers.google.com/maps/documentation/geocoding/)
  * [Yahoo! PlaceFinder](http://developer.yahoo.com/geo/placefinder/) - [docs](http://developer.yahoo.com/geo/placefinder/guide/index.html)
  * [Bing Maps (aka Virtual Earth)](http://www.microsoft.com/maps/) - [docs](http://msdn.microsoft.com/en-us/library/ff701715.aspx)

The API returns latitude/longitude coordinates and normalized address information.  This can be used to perform address validation, real time mapping of user-entered addresses, distance calculations, and much more.

### Simple Example

    IGeoCoder geoCoder = new GoogleGeoCoder("my-api-key");
    Address[] addresses = geoCoder.GeoCode("123 Main St");

It can also be used to return address information from latitude/longitude coordinates (aka reverse geocoding):

    IGeoCoder geoCoder = new YahooGeoCoder("my-app-ID");
    Address[] addresses = geoCoder.ReverseGeoCode(38.8976777, -77.036517);

How to Build from Source
------------------------

In order to compile the solution in Visual Studio, you must first run the build in MSBuild. The build process generates some files that are needed to compile in Visual Studio.

The easiest way to run the build is just to run build.bat. This will run a basic Debug build without running any tests.


### How to Build from Visual Studio 2010 Command Prompt

Here are some sample build commands (these should be run from the working copy root directory):

_Debug Build with tests_

    msbuild geocoding.build

_Debug Build without tests_

    msbuild geocoding.build /t:Compile

_Release Build_

    msbuild geocoding.build /p:Configuration=Release

_Copy & Merge Release Build to Output Dir (no tests)_

    msbuild geocoding.build /t:ILMerge /p:Configuration=Release

_Generate AssemblyInfoCommon file_

    msbuild geocoding.build /t:Version

_Generate App.config from template file_

    msbuild geocoding.build /t:CopyTemplateFiles


### Service Tests
You will need to generate API keys for each respective service to run the service tests. Edit App.config in Core.Tests and put in your API keys.