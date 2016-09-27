namespace Geocoding.Google
{
	public class GoogleComponentFilter
	{
		public string ComponentFilter { get; set; }

		public GoogleComponentFilter(string component, string value)
		{
			ComponentFilter = string.Format("{0}:{1}", component, value);
		}
	}
}
