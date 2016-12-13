using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Geocoding;
using Geocoding.Google;
using Geocoding.Microsoft;
using Geocoding.Yahoo;
using Geocoding.MapQuest;

namespace Example.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			var container = InitializeContainer();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

			AreaRegistration.RegisterAllAreas();
			RegisterRoutes(RouteTable.Routes);
		}

		IContainer InitializeContainer()
		{
			var builder = new ContainerBuilder();
			builder.RegisterControllers(Assembly.GetExecutingAssembly());

			//register your geocoder implementation here -- note: it will use the last one registered

			//http://msdn.microsoft.com/en-us/library/ff428642.aspx
			builder.Register(c => new BingMapsGeocoder("my-bing-maps-key")).As<IGeocoder>();

			//http://developer.yahoo.com/boss/geo/BOSS_Signup.pdf
			builder.Register(c => new YahooGeocoder("my-yahoo-consumer-key", "my-yahoo-consumer-secret")).As<IGeocoder>();

			builder.Register(c => new MapQuestGeocoder("mapquest-key") { UseOSM = true }).As<IGeocoder>();

			//https://developers.google.com/maps/documentation/javascript/tutorial#api_key
			//a server key is optional with Google
			builder.Register(c => new GoogleGeocoder
			{
				//ApiKey = "google-api-key-is-optional"
			}).As<IGeocoder>();

			return builder.Build();
		}

		void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}