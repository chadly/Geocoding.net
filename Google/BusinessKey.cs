using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Geocoding.Google
{
	/// <remarks>
	/// https://developers.google.com/maps/documentation/business/webservices/auth#business-specific_parameters
	/// </remarks>
	public class BusinessKey
	{
		public string ClientId { get; set; }
		public string SigningKey { get; set; }

		/// <summary>
		/// More details about channel
		/// https://developers.google.com/maps/documentation/directions/get-api-key
		/// https://developers.google.com/maps/premium/reports/usage-reports#channels
		/// </summary>
		private string channel;
		public string Channel
		{
			get
			{
				return channel;
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					return;
				}
				string formattedChannel = value.Trim().ToLower();
				if (Regex.IsMatch(formattedChannel, @"^[a-z_0-9.-]+$"))
				{
					channel = formattedChannel;
				}
				else
				{
					throw new ArgumentException("Must be an ASCII alphanumeric string; can include a period (.), underscore (_) and hyphen (-) character", "channel");
				}
			}
		}
		public bool HasChannel
		{
			get
			{
				return !string.IsNullOrEmpty(Channel);
			}
		}

		public BusinessKey(string clientId, string signingKey, string channel = null)
		{
			this.ClientId = CheckParam(clientId, "clientId");
			this.SigningKey = CheckParam(signingKey, "signingKey");
			this.Channel = channel;
		}

		string CheckParam(string value, string name)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentNullException(name, "Value cannot be null or empty.");

			return value.Trim();
		}

		public string GenerateSignature(string url)
		{
			var encoding = new ASCIIEncoding();
			var uri = new Uri(url);

			// converting key to bytes will throw an exception, need to replace '-' and '_' characters first.
			string usablePrivateKey = SigningKey.Replace("-", "+").Replace("_", "/");
			byte[] privateKeyBytes = Convert.FromBase64String(usablePrivateKey);

			byte[] encodedPathAndQueryBytes = encoding.GetBytes(uri.LocalPath + uri.Query);

			// compute the hash
			var algorithm = new HMACSHA1(privateKeyBytes);
			byte[] hash = algorithm.ComputeHash(encodedPathAndQueryBytes);

			// convert the bytes to string and make url-safe by replacing '+' and '/' characters
			string signature = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_");

			// Add the signature to the existing URI.
			return uri.Scheme + "://" + uri.Host + uri.LocalPath + uri.Query + "&signature=" + signature;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as BusinessKey);
		}

		public bool Equals(BusinessKey other)
		{
			if (other == null) return false;
			return ClientId.Equals(other.ClientId) && SigningKey.Equals(other.SigningKey);
		}

		public override int GetHashCode()
		{
			return ClientId.GetHashCode() ^ SigningKey.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("ClientId: {0}, SigningKey: {1}", ClientId, SigningKey);
		}
	}
}