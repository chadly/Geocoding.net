using System;
using System.Web.Mvc;
using GeoCoding;

namespace Example.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IGeoCoder geoCoder;

		public HomeController(IGeoCoder geoCoder)
		{
			this.geoCoder = geoCoder;
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Geocode(string address)
		{
			var addresses = geoCoder.GeoCode(address);
			return View("Index", addresses);
		}
	}
}