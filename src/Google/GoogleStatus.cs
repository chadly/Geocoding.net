using System;

namespace GeoCoding.Google
{
	public enum GoogleStatus
	{
		Error,
		Ok,
		ZeroResults,
		OverQueryLimit,
		RequestDenied,
		InvalidRequest
	}
}