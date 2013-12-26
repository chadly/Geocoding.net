using System;
using System.Linq;
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
			if (String.IsNullOrEmpty(address))
				return View("Index");

			var addresses = geoCoder.GeoCode(address).ToArray();
			return View("Index", addresses);
		}
	}
}