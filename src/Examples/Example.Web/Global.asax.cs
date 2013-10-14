using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using GeoCoding;
using GeoCoding.Google;
using GeoCoding.Microsoft;
using GeoCoding.Yahoo;

namespace Example.Web
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			var container = InitializeContainer();

			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

			RegisterRoutes(RouteTable.Routes);
		}

		private IContainer InitializeContainer()
		{
			var builder = new ContainerBuilder();
			builder.RegisterControllers(Assembly.GetExecutingAssembly());

			//register your geocoder implementation here -- note: it will use the last one registered
			builder.Register(c => new BingMapsGeoCoder("my-bing-maps-key")).As<IGeoCoder>();
			builder.Register(c => new YahooGeoCoder("my-yahoo-app-id")).As<IGeoCoder>();
			builder.Register(c => new GoogleGeoCoder()).As<IGeoCoder>();

			return builder.Build();
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
	}
}