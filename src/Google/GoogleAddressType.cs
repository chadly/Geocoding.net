using System;

namespace Geocoding.Google
{
	/// <remarks>
	/// http://code.google.com/apis/maps/documentation/geocoding/#Types
	/// </remarks>
	public enum GoogleAddressType
	{
		Unknown,
		StreetAddress,
		Route,
		Intersection,
		Political,
		Country,
		AdministrativeAreaLevel1,
		AdministrativeAreaLevel2,
		AdministrativeAreaLevel3,
		ColloquialArea,
		Locality,
		SubLocality,
		Neighborhood,
		Premise,
		Subpremise,
		PostalCode,
		NaturalFeature,
		Airport,
		Park,
		PointOfInterest,
		PostBox,
		StreetNumber,
		Floor,
		Room
	}
}