namespace Geocoding.UsCensusBureau
{
	public class UsCensusBureauAddress : Address
	{
		public UsCensusBureauAddress(string formattedAddress, Location coordinates) 
			: base(formattedAddress, coordinates, UsCensusBureauConstants.Provider)
		{
            
		}
	}
}
