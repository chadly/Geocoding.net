using System;

namespace GeoCoding.Google
{
    public enum GoogleStatusCode
    {
        Success = 200,
        ServerError = 500,
        MissingAddress = 601,
        UnknownAddress = 602,
        UnavailableAddress = 603,
        BadKey = 610
    }
}
