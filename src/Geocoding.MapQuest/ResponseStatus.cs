namespace Geocoding.MapQuest
{
	public enum ResponseStatus : int
	{
		Ok = 0,
		OkBatch = 100,
		ErrorInput = 400,
		ErrorAccountKey = 403,
		ErrorUnknown = 500,
	}
}