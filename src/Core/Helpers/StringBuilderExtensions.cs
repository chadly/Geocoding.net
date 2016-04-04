using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Geocoding
{
    public static class StringBuilderExtensions
    {
        #region Public Methods

        public static string StringJoin(this IEnumerable<string> list, string seperator)
        {
            var result = default(string);
            if (list != null)
            {
                result = string.Join(seperator, list);
            }

            return result;
        }

        public static void AddParameterIfNotNullOrEmpty(this StringBuilder sb, string key, string value)
        {
            if (false == string.IsNullOrEmpty(value))
            {
                var lastIndex = sb.Length - 1;

                if (lastIndex > 0 && sb[lastIndex] != '?') { 
                    sb.Append("&");
                }
                sb.Append(key).Append("=").Append(WebUtility.UrlEncode(value));
            }
        }


        public static void AddIfNotNullOrEmpty(this StringBuilder sb, string value, string delimeter)
        {
            if (false == string.IsNullOrEmpty(value))
            {
                sb.Append(value).Append(delimeter);
            }
        }
        #endregion Public Methods
    }
}