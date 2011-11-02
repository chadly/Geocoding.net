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
        UnknownDirections = 604,
        BadKey = 610,
        TooManyQueries = 620
    }
}
