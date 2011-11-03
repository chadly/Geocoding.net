using System;
using System.Net;

namespace GeoCoding.Microsoft
{
	public static class LocalIPAddress
	{
		public static string Current
		{
			get
			{
				var host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (var ip in host.AddressList)
				{
					if (ip.AddressFamily.ToString() == "InterNetwork")
					{
						return ip.ToString();
					}
				}
				return "127.0.0.1";
			}
		}
	}
}