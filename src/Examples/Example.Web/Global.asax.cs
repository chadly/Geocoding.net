using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac.Builder;
using Autofac.Integration.Web;
using Autofac.Integration.Web.Mvc;
using GeoCoding;
using GeoCoding.Google;
using GeoCoding.VirtualEarth;
using GeoCoding.Yahoo;

namespace Example.Web
{
	public class MvcApplication : System.Web.HttpApplication, IContainerProviderAccessor
	{
		private static IContainerProvider containerProvider;

		public IContainerProvider ContainerProvider
		{
			get { return containerProvider; }
		}

		private void InitializeContainer()
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule(new AutofacControllerModule(Assembly.GetExecutingAssembly()));

			//register your geocoder implementation here -- note: it will use the last one registered
			builder.Register(c => new VirtualEarthGeoCoder("my-virtual-earth-username", "my-virtual-earth-password")).As<IGeoCoder>();
			builder.Register(c => new YahooGeoCoder("my-yahoo-app-id")).As<IGeoCoder>();
			builder.Register(c => new GoogleGeoCoder("my-google-api-key")).As<IGeoCoder>();

			containerProvider = new ContainerProvider(builder.Build());
			ControllerBuilder.Current.SetControllerFactory(new AutofacControllerFactory(ContainerProvider));
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default",                                              // Route name
				"{controller}/{action}/{id}",                           // URL with parameters
				new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
			);
		}

		protected void Application_Start()
		{
			InitializeContainer();
			RegisterRoutes(RouteTable.Routes);
		}
	}
}