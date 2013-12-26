using System;
using System.Linq;
using System.Web.Mvc;
using Geocoding;

namespace Example.Web.Controllers
{
	public class HomeController : Controller
	{
		readonly IGeocoder geoCoder;

		public HomeController(IGeocoder geoCoder)
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

			var addresses = geoCoder.Geocode(address).ToArray();
			return View("Index", addresses);
		}
	}
}