using System;

namespace GeoCoding.Microsoft
{
	public enum AuthenticationResultCode
	{
		None,
		NoCredentials,
		ValidCredentials,
		InvalidCredentials,
		CredentialsExpired,
		NotAuthorized,
	}
}