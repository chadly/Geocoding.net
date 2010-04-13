using System;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using GeoCoding.Services.VirtualEarth.Imagery;
using GeoCoding.Services.VirtualEarth.Token;

namespace GeoCoding.Services.VirtualEarth
{
	public class VirtualEarthServiceFactory : IDisposable
	{
		private readonly ChannelFactory<IGeocodeService> geocodeServiceFactory;
		private readonly ChannelFactory<CommonServiceSoap> tokenServiceFactory;
		private readonly ChannelFactory<IImageryService> imageryServiceFactory;

		public ChannelFactory<IGeocodeService> GeocodeServiceFactory
		{
			get { return geocodeServiceFactory; }
		}

		public ChannelFactory<CommonServiceSoap> TokenServiceFactory
		{
			get { return tokenServiceFactory; }
		}

		public ChannelFactory<IImageryService> ImageryServiceFactory
		{
			get { return imageryServiceFactory; }
		}

		public VirtualEarthServiceFactory(string username, string password)
			: this(username, password, false) { }

		public VirtualEarthServiceFactory(string username, string password, bool useStaging)
		{
			this.geocodeServiceFactory = CreateGeocodeChannelFactory(useStaging);
			this.tokenServiceFactory = CreateTokenChannelFactory(username, password, useStaging);
			this.imageryServiceFactory = CreateImageryChannelFactory(useStaging);
		}

		private ChannelFactory<IGeocodeService> CreateGeocodeChannelFactory(bool useStaging)
		{
			string endPoint = useStaging ?
				"http://staging.dev.virtualearth.net/webservices/v1/geocodeservice/GeocodeService.svc" :
				"http://dev.virtualearth.net/webservices/v1/geocodeservice/GeocodeService.svc";

			return new ChannelFactory<IGeocodeService>(new BasicHttpBinding(), new EndpointAddress(endPoint));
		}

		private ChannelFactory<CommonServiceSoap> CreateTokenChannelFactory(string username, string password, bool useStaging)
		{
			string endPoint = useStaging ?
				"https://staging.common.virtualearth.net/find-30/common.asmx" :
				"https://common.virtualearth.net/find-30/common.asmx";

			var binding = new BasicHttpBinding();
			binding.Security.Mode = BasicHttpSecurityMode.Transport;
			binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Digest;

			var factory = new ChannelFactory<CommonServiceSoap>(binding, new EndpointAddress(endPoint));
			factory.Credentials.HttpDigest.ClientCredential = new NetworkCredential(username, password);
			factory.Credentials.HttpDigest.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;

			return factory;
		}

		private ChannelFactory<IImageryService> CreateImageryChannelFactory(bool useStaging)
		{
			string endPoint = useStaging ?
				"http://staging.dev.virtualearth.net/webservices/v1/imageryservice/imageryservice.svc" :
				"http://dev.virtualearth.net/webservices/v1/imageryservice/imageryservice.svc";

			return new ChannelFactory<IImageryService>(new BasicHttpBinding(), new EndpointAddress(endPoint));
		}

		public IGeocodeService CreateGeocodeService()
		{
			return GeocodeServiceFactory.CreateChannel();
		}

		public CommonServiceSoap CreateTokenService()
		{
			return TokenServiceFactory.CreateChannel();
		}

		public void Dispose()
		{
			if (geocodeServiceFactory != null)
				((IDisposable)geocodeServiceFactory).Dispose();

			if (tokenServiceFactory != null)
				((IDisposable)tokenServiceFactory).Dispose();
		}
	}
}